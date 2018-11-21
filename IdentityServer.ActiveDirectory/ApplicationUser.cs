using Microsoft.AspNetCore.Identity;

namespace IdentityServer.ActiveDirectory
{
    public class ApplicationUser : IdentityUser
    {
        public bool Enabled { get; set; } = true;
    }}