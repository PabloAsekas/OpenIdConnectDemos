using System;
using System.ComponentModel.DataAnnotations;

namespace Host.Advanced.Models.U2F
{
    public class RegisterU2FViewModel
    {
        [Display(Name = "Challenge")]
        public string Challenge { get; set; }

        [Display(Name = "Version")]
        public string Version { get; set; }

        [Display(Name = "App ID")]
        public string AppId { get; set; }

        [Display(Name = "Device Response")]
        public string DeviceResponse { get; set; }

        public string DeviceName { get; set; }
    }
}