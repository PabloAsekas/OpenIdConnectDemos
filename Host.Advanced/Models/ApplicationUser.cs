using System.Collections.Generic;
using Host.Advanced.Data.U2F;
using Microsoft.AspNetCore.Identity;

namespace Host.Advanced.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Device> DeviceRegistrations { get; set; }

        public virtual ICollection<AuthenticationRequest> AuthenticationRequest { get; set; }
    }
}
