using System.Collections.Generic;
using Host.Advanced.Models.U2F;

namespace Host.Advanced.Models.ManageViewModels
{
    public class TwoFactorAuthenticationViewModel
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }

        public List<DeviceModel> Devices { get; set; }
    }
}
