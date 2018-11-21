namespace IdentityServer.ActiveDirectory
{
    public interface ISecurityManagerService
    {
        bool ValidateCredentials(string username, string password);

        ApplicationUser GetIdentityInfoByUsername(string username);

        ApplicationUser GetIdentityInfoBySubject(string sid);
    }
}