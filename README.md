# OpenIdConnectDemos

This project contains the demos explained in the event Commit 2018 Madrid.

Every web host is based on the standard template _ASP.NET Core Web Application_ within Visual Studio.

Before running the projects, a couple of steps should be done:

1. Create the database for each project (Host & Host.Advance). Execute the following command from a console within each directory:
```
dotnet ef database update -c ApplicationDbContext
```
2. Seed each the database with users data. The parameter seed will be caught in the Program entry point. Execute the following command from a console within each directory:
```
dotnet run -- seed
```

## Demo 1
### Create our first SSO server

This project will act as an identity server implementing the protocol OpenId Connect for our applications. To accomplish this we'll use the library [Identity Server 4](https://github.com/IdentityServer/IdentityServer4) as well as [ASP.NET Core Identity](https://github.com/aspnet/identity) available as nuget packages.

The project, in addition to being based on the standard template, contains a Quickstart template from [IS4 Samples](https://github.com/IdentityServer/IdentityServer4.Samples) repo.

The relevant code for this demo is in Startup class where everything is configured.

## Demo 2
### Extend the SSO server to query ActiveDirectory
______________________________________________________
> Please, take into account that for this demo you need an Active Directory implementation. You can use a virtual machine, a real environment or if neither of this options it's feasible for you, try this LDAP implementation: https://nordes.github.io/#/Articles/howto-openldap-with-contoso-users

In order to connect our SSO to AD, we'll replace two classes from ASP.Net Identity library: `UserManager` responsible among other tasks to validate the user password and `UserClaimsFactory` this one used to adapt the final list of claims to include in the emitted token. Additionally, we need a class to manage the AD context and the operations to perform in the directory. 

You can find all this code in the project IdentityServer.ActiveDirectory within the solution.

Finally, you only need to change the Startup class in Host project a bit to introduce the replaced classes. Replace the line (in identity server config)
```csharp
.AddAspNetIdentity<ApplicationUser>();
```
with this code
```csharp
.AddActiveDirectoryIdentity<ApplicationUser>(options =>
    {
        options.DomainName = Configuration.GetSection("ActiveDirectory")?["DomainName"];
        options.DomainUser = Configuration.GetSection("ActiveDirectory")?["DomainUser"];
        options.DomainCryptPassword = Configuration.GetSection("ActiveDirectory")?["DomainPassword"];
    });
```

At this point, we rely on the [user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) feature in dotnet core to save confidential info like user and password to connect to AD. Use the following command to manage them:
```bash
dotnet user-secrets <command>
```

### Connect the SSO server with Social Login providers
______________________________________________________

The code to do this it's simple and we'll use the library included in the standard ASP.Net core libraries [Microsoft.AspNetCore.Authentication](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication) (Usually you don't need to add this package manually because it's included in the metapackage Microsoft.AspNetCore.All).

Add this code to the end of the method `ConfigureServices` in `Startup` class:
```csharp
services.AddAuthentication()
    //Google Developer Console https://console.cloud.google.com/apis/credentials
    .AddGoogle(options =>
    {
        options.ClientId = "813017584167-dqd2qoo9oau2khmg0binpefnjgq5udar.apps.googleusercontent.com";
        options.ClientSecret = "Vh0P8HMQbIDJ4E2PK5XEkiQj";
    })
    //Microsoft Developer Console https://apps.dev.microsoft.com/#/appList
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = "2d8b1dbd-7bb4-4098-b974-541523a01565";
        options.ClientSecret = "pbmsoaSCU59625@^mPNHJ^+";
    });
```

> Notice how we're using a client-id and a client-secret (for demo purposes) previously configured in the corresponding dev consoles for each provider. If you want to use any other provider, look for it because it's mandatory to configure this values.

Now if you start the project again, to make login use a valid username/password of your AD infrastructure.

## Demo 3
### Connect an App with our SSO server
______________________________________________________
For this demo we have two additional projects within Clients folder. A web application and a web api to demonstrate the multiple purposes of our SSO server.

Relevant parts of code here are:
* Web App (ECApp): The Startup class has all the configuration needed to connect to an SSO using the extension method `AddOpenIdConnect` included in the standard packages for ASP.Net core.<br/>This configuration it's defined in the class `Host.Configuration.Clients` and added in the InMemory repository through the extension method `.AddInMemoryClients(Clients.Get())` as well as the resources needed to startup the SSO server.
* Web API (ECApi): In this case, we'll use the library to [IdentityServer4.AccessTokenValidation](https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation) to parse and validate the tokens receive in every call to our API.<br/>We'll also set up an authorization policy to require a specific scope. You can see the policy in action in the `FilmsController` class: adding the attribute `[Authorize("ECApiPolicy")]` with the policy name.

## Demo 4
An advance SSO (Host.Advance project) it's provided with a second-factor authentication enabled. Basically, it's our first implementation with some classes to manage U2F devices.

The original demo was made with a set of Yubikeys, but for testing purposes, you have software alternatives in the form of a mobile app and browser plugin to simulate a physical key, i.e. [Krypton Authenticator](https://krypt.co/) or any other.
Notice here how you should manage the 2FA required in the login process: in class `AccountController`, `Login` action (for POST method) and `LoginWith2fa`.

The process consists of a challenge generated for the browser to interact with the user, waiting for a valid response to that challenge. Here we use the u2f-api.js from the [U2F specification reference project](https://github.com/google/u2f-ref-code) and a very useful implementation in dotnet core with the nuget package [U2F.Core](https://github.com/brucedog/U2F_Core)