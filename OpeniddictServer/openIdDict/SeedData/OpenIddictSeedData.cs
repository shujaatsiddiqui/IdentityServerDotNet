using OpenIddict.Abstractions;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
