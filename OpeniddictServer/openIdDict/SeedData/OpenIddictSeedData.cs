using OpenIddict.Abstractions;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using IdentityModel;

public static class OpenIddictSeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // Register the SPA client
        if (await manager.FindByClientIdAsync("spaClient") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "spaClient",
                DisplayName = "Single Page Application Client",
                Permissions =
                {
                    // Allow authorization code flow
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                
                // Allow the `code` response type
                OpenIddictConstants.Permissions.ResponseTypes.Code,

                // Allow access to OpenID Connect scopes
                "scp:openid",
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,

                // Allow endpoints
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Logout,

                    "scp:el.manage" // Custom scope
                },
                RedirectUris = { new Uri("http://localhost:4200/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("http://localhost:4200/signout-callback-oidc") },
                Requirements =
                {
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange // PKCE for SPA
                }
            });
        }

        if (await manager.FindByClientIdAsync("vcmsAPI") is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "vcmsAPI",
                DisplayName = "vcsm api resource",
                ClientSecret = "vcmssecret".ToSha256(), 
                Permissions =
                {
                    // Allow authorization code flow
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    // Allow access to the `vcms.manage` scope
                    "scp:vcms.manage", 
                    // Allow token endpoint
                    OpenIddictConstants.Permissions.Endpoints.Token
                }
            });
        }



        // Optional: Register custom scopes if not already present
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        if (await scopeManager.FindByNameAsync("el.manage") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "el.manage",
                Resources = { "resource-server" } // Add your resource identifiers here
            });
        }

        if (await scopeManager.FindByNameAsync("vcms.manage") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "vcms.manage",
                Resources = { "vcmsAPI" }
                // Associate the scope with the API resource 
            });
        }

    }
}
