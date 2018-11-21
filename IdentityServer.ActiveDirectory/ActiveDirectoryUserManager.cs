using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.ActiveDirectory
{
    public class ActiveDirectoryUserManager : AspNetUserManager<ApplicationUser>
    {
        private readonly ISecurityManagerService securityManager;

        public ActiveDirectoryUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger,
            ISecurityManagerService securityManager) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            this.securityManager = securityManager;
        }

        public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return Task.FromResult(securityManager.ValidateCredentials(user.UserName, password));
        }

        public override async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return securityManager.GetIdentityInfoByUsername(userName) ?? await base.FindByNameAsync(userName);
        }

        public override async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return securityManager.GetIdentityInfoBySubject(userId) ?? await base.FindByIdAsync(userId);
        }
    }
}