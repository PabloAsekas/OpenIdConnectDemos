using System.Collections.Generic;
using System.Threading.Tasks;
using Host.Advanced.Data.U2F;
using Microsoft.AspNetCore.Identity;
using U2F.Core.Models;

namespace Host.Advanced.Services
{
    public interface IU2FManagerService
    {
        Task<ServerRegisterResponse> StartU2FRegistration(string username);

        Task<bool> FinishU2FRegistration(string userName, string deviceResponse, string deviceName,
            StartedRegistration startedRegistration);
        
        Task<List<ServerChallenge>> StartTwoFactorU2FSignInAsync(string username);

        Task<SignInResult> FinishTwoFactorU2FSignInAsync(string userName, string deviceResponse, bool rememberMe = false, bool rememberMachine = false);

        Task RemoveDeviceAsync(string username, int deviceId);
    }
}