using System;
using System.Buffers.Text;
using System.DirectoryServices.AccountManagement;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.ActiveDirectory
{
    public class ActiveDirectoryManagerService : IDisposable, ISecurityManagerService
    {
        private readonly ILogger<ActiveDirectoryManagerService> logger;

        private readonly ActiveDirectoryIdentityOptions options = new ActiveDirectoryIdentityOptions();

        private readonly Lazy<PrincipalContext> principalContext;

        public PrincipalContext PrincipalContext => principalContext.Value;

        public ActiveDirectoryManagerService(ILogger<ActiveDirectoryManagerService> logger, IConfigureOptions<ActiveDirectoryIdentityOptions> options)
        {
            this.logger = logger;
            options.Configure(this.options);
            principalContext = new Lazy<PrincipalContext>(InitContext, true);
        }

        private PrincipalContext InitContext()
        {
            try
            {
                return new PrincipalContext(ContextType.Domain, options.DomainName, options.DomainUser, options.DomainCryptPassword);
            }
            catch (PrincipalException ex)
            {
                logger.LogError(ex, $"Error creating a domain context into {options.DomainName} with user {options.DomainUser}.");
            }

            return null;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return PrincipalContext != null &&
                   PrincipalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
        }

        public ApplicationUser GetIdentityInfoByUsername(string username)
        {
            var principal = GetPrincipal(IdentityType.SamAccountName, username);
            return principal?.ToApplicationUser();
        }

        public ApplicationUser GetIdentityInfoBySubject(string sid)
        {
            UserPrincipal principal = null;
            try
            {
                principal = GetPrincipal(IdentityType.Sid, sid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred looking for sid {sid}: {ex.Message}");
            }
            return principal?.ToApplicationUser();
        }

        public void Dispose()
        {
            principalContext?.Value.Dispose();
        }

        private UserPrincipal GetPrincipal(IdentityType type, string account)
        {
            if (PrincipalContext != null)
            {
                try
                {
                    return UserPrincipal.FindByIdentity(PrincipalContext, type, account);
                }
                catch (PrincipalException ex)
                {
                    logger.LogError(ex, $"Error querying domain for user: [{account}]");
                }
            }

            return null;
        }
    }

    public static class ActiveDirectoryExtensions
    {
        public static ApplicationUser ToApplicationUser(this UserPrincipal userPrincipal)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = userPrincipal.SamAccountName,
                Email = userPrincipal.EmailAddress,
                Id = userPrincipal.Sid.ToString(),
                Enabled = userPrincipal.Enabled == true,
                EmailConfirmed = true,
                PhoneNumber= userPrincipal.VoiceTelephoneNumber,
                PhoneNumberConfirmed = true
            };
            if (userPrincipal.LastPasswordSet.HasValue)
            {
                var hash = SHA256.Create();
                applicationUser.SecurityStamp = Convert.ToBase64String(hash.ComputeHash(BitConverter.GetBytes(userPrincipal.LastPasswordSet.Value.Ticks)));
            }

            return applicationUser;
        }
    }
}