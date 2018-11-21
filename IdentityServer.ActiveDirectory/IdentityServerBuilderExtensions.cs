using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer.ActiveDirectory
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddActiveDirectoryIdentity<TUser>(this IIdentityServerBuilder builder, Action<ActiveDirectoryIdentityOptions> adOptions)
            where TUser : class 
        {
            builder.Services.Configure(adOptions);

            builder.Services.Replace(new ServiceDescriptor(typeof(IUserClaimsPrincipalFactory<ApplicationUser>), typeof(UserClaimsFactory), ServiceLifetime.Transient));
            builder.Services.Replace(new ServiceDescriptor(typeof(UserManager<ApplicationUser>), typeof(ActiveDirectoryUserManager), ServiceLifetime.Transient));

            builder.Services.AddSingleton<ISecurityManagerService, ActiveDirectoryManagerService>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });


            builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
            {
                opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            });

            builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, cookie =>
            {
                // we need to disable to allow iframe for authorize requests
                cookie.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            builder.AddResourceOwnerValidator<ResourceOwnerPasswordValidator<TUser>>();
            builder.AddProfileService<ProfileService>();

            return builder;
        }

        public static IEnumerable<Claim> ToClaims(this ApplicationUser user)
        {
            var claims = new List<Claim> { new Claim(JwtClaimTypes.Subject, user.Id) };
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean));

            }
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));
                claims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean));
            }
            return claims;
        }
    }
}