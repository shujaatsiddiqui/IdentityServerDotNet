using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;
using Microsoft.Extensions.DependencyInjection;
using IdentityModel;
using System.Linq;
using System.Threading.Tasks;
using CourtAuth.IdentityServer.Data;

public static class IdentitySeedData
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure the "angella" user
        var angella = await userMgr.FindByNameAsync("angella");
        if (angella == null)
        {
            angella = new ApplicationUser
            {
                UserName = "angella",
                Email = "angella.freeman@email.com",
                EmailConfirmed = true
            };

            var result = await userMgr.CreateAsync(angella, "Pass123$");
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            // Add claims to the user
            result = userMgr.AddClaimsAsync(
                angella,
                new[]
                {
                    new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                    new Claim(JwtClaimTypes.GivenName, "Angella"),
                    new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                    new Claim(JwtClaimTypes.WebSite, "http://angellafreeman.com"),
                    new Claim("location", "somewhere")
                }
            ).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}
