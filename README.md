# OpenIdConnectDemos

This project contains the demos explained in the event Commit 2018 Madrid.

Every web host is based in the standard template _ASP.NET Core Web Application_ within Visual Studio.

Before running the projects, a couple of steps should be done:

1. Create the database for each project (Host & Host.Advance). Execute the following command from a console within each directory:
```
dotnet ef database update -c ApplicationDbContext
```
2. Seed each the database with users data. The parameter seed will be catched in the Program entry point. Execute the following command from a console within each directory:
```
dotnet run -- seed
```

## Demo 1
### Create our first SSO server

This project will act as an identity server implementing the protocol OpenId Connect for our applications. To accomplish this we'll use the library <a href="https://github.com/IdentityServer/IdentityServer4">Identity Server 4</a> as well as <a href="https://github.com/aspnet/identity">ASP.NET Core Identity</a> available as nuget packages.

The project, in addition to be based on the standard template, contains a Quickstart template from <a href="https://github.com/IdentityServer/IdentityServer4.Samples">IS4 Samples</a> repo.

The relevant code for this demo is in Startup class where the everything is configured.