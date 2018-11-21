using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Host.Advanced.Configuration;
using Host.Advanced.Data;
using Host.Advanced.Data.U2F;
using Host.Advanced.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using U2F.Core.Models;
using U2F.Core.Utils;

namespace Host.Advanced.Services
{
    public class U2FManagerService : IU2FManagerService
    {
        private readonly U2FManagerOptions options = new U2FManagerOptions();
        
        private readonly ApplicationDbContext dataContext;

        private readonly UserManager<ApplicationUser> userManager;
        
        private readonly SignInManager<ApplicationUser> signInManager;

        public U2FManagerService(ApplicationDbContext userRepository, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IConfigureOptions<U2FManagerOptions> options)
        {
            dataContext = userRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            options.Configure(this.options);
        }
        
        public async Task<ServerRegisterResponse> StartU2FRegistration(string username)
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName.Equals(username));
            if (user == null)
            {
                return null;
            }

            var startedRegistration = U2F.Core.Crypto.U2F.StartRegistration(options.SafeAppUri);

            user.AuthenticationRequest = user.AuthenticationRequest ?? new List<AuthenticationRequest>();
            user.DeviceRegistrations = user.DeviceRegistrations ?? new List<Device>();

            user.AuthenticationRequest.ToList().ForEach(a => dataContext.Remove(a));
            user.AuthenticationRequest.Add(
                new AuthenticationRequest
                {
                    AppId = startedRegistration.AppId,
                    Challenge = startedRegistration.Challenge,
                    Version = U2F.Core.Crypto.U2F.U2FVersion
                });


            await dataContext.SaveChangesAsync();

            return new ServerRegisterResponse
            {
                AppId = startedRegistration.AppId,
                Challenge = startedRegistration.Challenge,
                Version = startedRegistration.Version
            };
        }

        public async Task<List<ServerChallenge>> StartTwoFactorU2FSignInAsync(string userName)
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName.Equals(userName));

            if (user?.DeviceRegistrations?.Any() != true)
            {
                return null;
            }

            user.AuthenticationRequest.ToList().ForEach(a => dataContext.Remove(a));
            var challenge = U2F.Core.Crypto.U2F.GenerateChallenge();

            var serverChallenges = new List<ServerChallenge>();
            foreach (var registeredDevice in user.DeviceRegistrations)
            {
                serverChallenges.Add(new ServerChallenge
                {
                    appId = options.SafeAppUri,
                    challenge = challenge,
                    keyHandle = registeredDevice.KeyHandle.ByteArrayToBase64String(),
                    version = U2F.Core.Crypto.U2F.U2FVersion
                });
                user.AuthenticationRequest.Add(
                    new AuthenticationRequest
                    {
                        AppId = options.SafeAppUri,
                        Challenge = challenge,
                        KeyHandle = registeredDevice.KeyHandle.ByteArrayToBase64String(),
                        Version = U2F.Core.Crypto.U2F.U2FVersion
                    });
            }
            await dataContext.SaveChangesAsync();
            return serverChallenges;
        }

        public async Task<bool> FinishU2FRegistration(string userName, string deviceResponse, string deviceName, StartedRegistration startedRegistration)
        {
            if (string.IsNullOrWhiteSpace(deviceResponse))
            {
                return false;
            }

            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName.Equals(userName));
            if (user?.AuthenticationRequest?.Any() != true)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(deviceName))
            {
                deviceName = Guid.NewGuid().ToString();
            }

            var registerResponse = BaseModel.FromJson<RegisterResponse>(deviceResponse);
            var registration = U2F.Core.Crypto.U2F.FinishRegistration(startedRegistration, registerResponse);

            user.AuthenticationRequest.ToList().ForEach(a => dataContext.Remove(a));
            user.DeviceRegistrations.Add(new Device
            {
                AttestationCert = registration.AttestationCert,
                Counter = Convert.ToInt32(registration.Counter),
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                KeyHandle = registration.KeyHandle,
                PublicKey = registration.PublicKey,
                DeviceName = deviceName
            });
            var result = await dataContext.SaveChangesAsync();
            await userManager.SetTwoFactorEnabledAsync(user, true);

            return result > 0;
        }

        public async Task<SignInResult> FinishTwoFactorU2FSignInAsync(string userName, string deviceResponse, bool rememberMe = false, bool rememberMachine = false)
        {            
            var authenticateResponse = BaseModel.FromJson<AuthenticateResponse>(deviceResponse);
            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName.Equals(userName));
            var device = user.DeviceRegistrations.FirstOrDefault(f => f.KeyHandle.SequenceEqual(authenticateResponse.KeyHandle.Base64StringToByteArray()));

            if (device == null || user.AuthenticationRequest?.Any() != true)
            {
                return SignInResult.Failed;
            }

            // User will have a authentication request for each device they have registered so get the one that matches the device key handle
            var authenticationRequest = user.AuthenticationRequest.First(f => f.KeyHandle.Equals(authenticateResponse.KeyHandle));
            var registration = new DeviceRegistration(device.KeyHandle, device.PublicKey, device.AttestationCert, Convert.ToUInt32(device.Counter));
            var authentication = new StartedAuthentication(authenticationRequest.Challenge, authenticationRequest.AppId, authenticationRequest.KeyHandle);

            U2F.Core.Crypto.U2F.FinishAuthentication(authentication, authenticateResponse, registration);

            if (rememberMachine)
            {
                await signInManager.RememberTwoFactorClientAsync(user);
            }
            
            await signInManager.SignInAsync(user, new AuthenticationProperties{IsPersistent = rememberMe}, "U2F");

            user.AuthenticationRequest.ToList().ForEach(a => dataContext.Remove(a));

            device.Counter = Convert.ToInt32(registration.Counter);
            device.UpdatedOn = DateTime.Now;

            var result = await dataContext.SaveChangesAsync();

            return result > 0 ? SignInResult.Success : SignInResult.Failed;
        }

        public async Task RemoveDeviceAsync(string username, int deviceId)
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.NormalizedUserName.Equals(username));

            user.AuthenticationRequest.ToList().ForEach(a => dataContext.Remove(a));
            dataContext.Remove(user.DeviceRegistrations.First(d => d.Id == deviceId));
            await dataContext.SaveChangesAsync();

            if (await userManager.GetAuthenticatorKeyAsync(user) == null && !user.DeviceRegistrations.Any())
            {
                await userManager.SetTwoFactorEnabledAsync(user, false);
            }
        }
    }
}