using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.ActiveDirectory
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory;

        public ProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            this.userManager = userManager;
            this.claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await userManager.FindByIdAsync(sub);

            var claimsPrincipal = await claimsFactory.CreateAsync(user);

            context.AddRequestedClaims(claimsPrincipal.Claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await userManager.FindByIdAsync(sub);
            context.IsActive = user != null && user.Enabled;
        }
    }
}