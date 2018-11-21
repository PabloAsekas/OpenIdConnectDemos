using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.ActiveDirectory
{
    public class UserClaimsFactory : IUserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IdentityOptions options;

        public UserClaimsFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
        {
            this.userManager = userManager;
            options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof (optionsAccessor));
        }

        public async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var id = new ClaimsIdentity("Identity.Application", options.ClaimsIdentity.UserNameClaimType, options.ClaimsIdentity.RoleClaimType);

            id.AddClaims(user.ToClaims());

            if (userManager.SupportsUserClaim)
            {
                var localClaims = (await userManager.GetClaimsAsync(user)).ToList();
                localClaims.ForEach(lc =>
                {
                    if (id.Claims.All(c => lc.Type != c.Type))
                    {
                        id.AddClaim(new Claim(lc.Type, lc.Value));
                    }
                });
            }

            if (id.Claims.All(c => c.Type != options.ClaimsIdentity.UserIdClaimType))
            {
                id.AddClaim(new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id));
            }
            if (id.Claims.All(c => c.Type != JwtClaimTypes.Name))
            {
                id.AddClaim(new Claim(JwtClaimTypes.Name, user.UserName));
            }
            if (id.Claims.All(c => c.Type != JwtClaimTypes.PreferredUserName))
            {
                id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
            }

            id.AddClaim(new Claim(options.ClaimsIdentity.SecurityStampClaimType, user.SecurityStamp));

            return new ClaimsPrincipal(id);

        }
    }
}