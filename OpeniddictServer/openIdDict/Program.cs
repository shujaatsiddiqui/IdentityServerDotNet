using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using CourtAuth.IdentityServer.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System;
using CourtAuth.IdentityServer;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;
using CourtAuth.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Get configuration and connection strings
var configuration = builder.Configuration;
var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCors(options => { options.AddPolicy("AllowAngularApp", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });
// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add the database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(defaultConnString, b => b.MigrationsAssembly(assembly));
    options.UseOpenIddict(); // Register OpenIddict
});

// Enable developer exception page for EF Core
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Add Quartz.NET services
builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddAuthentication()
    .AddOpenIdConnect("AAD", "Microsoft", options =>
    {
        var azureAdOptions = builder.Configuration.GetSection("AzureAd").Get<AzureAdOptions>();

        options.Authority = azureAdOptions.Authority;
        options.ClientId = azureAdOptions.ClientId;
        options.ClientSecret = azureAdOptions.ClientSecret;
        options.CallbackPath = azureAdOptions.CallbackPath;

        options.ResponseType = "code"; // Authorization Code flow
        options.SaveTokens = true;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.RequireHttpsMetadata = true;
    });
// Configure OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
        options.UseQuartz();
    })
    //.AddClient(options =>
    //{
    //    options.AllowAuthorizationCodeFlow();
    //    options.AddDevelopmentEncryptionCertificate()
    //           .AddDevelopmentSigningCertificate();
    //    options.UseAspNetCore()
    //           .EnableStatusCodePagesIntegration()
    //           .EnableRedirectionEndpointPassthrough();
    //    options.UseSystemNetHttp()
    //           .SetProductInformation(typeof(Program).Assembly);
    //    options.UseWebProviders()
    //           .AddGitHub(options =>
    //           {
    //               options.SetClientId("c4ade52327b01ddacff3")
    //                      .SetClientSecret("da6bed851b75e317bf6b2cb67013679d9467c122")
    //                      .SetRedirectUri("callback/login/github");
    //           });
    //})
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetLogoutEndpointUris("connect/logout")
               .SetTokenEndpointUris("connect/token")
               .SetUserinfoEndpointUris("connect/userinfo");

        var issuer = configuration["Authentication:Issuer"];
        options.SetIssuer(issuer);

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles,Scopes.OpenId);

        options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        // Enable the client credentials flow.
        options.AllowClientCredentialsFlow();
        options.AllowRefreshTokenFlow();

        options.DisableAccessTokenEncryption();


        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough()
               .EnableStatusCodePagesIntegration();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Add hosted worker for background seeding
builder.Services.AddHostedService<Worker>();

// Build the app
var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming Request: {context.Request.Path}, RedirectUri: {context.Request.Query["redirect_uri"]}");
    await next();
});
// Ensure database creation and data seeding
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (await context.Database.EnsureCreatedAsync())
    {
        Console.WriteLine("Database created successfully.");
    }
    else
    {
        Console.WriteLine("Database already exists.");
    }

#if DEBUG
    await OpenIddictSeedData.SeedAsync(scope.ServiceProvider);
    await IdentitySeedData.SeedUsers(scope.ServiceProvider);
#endif
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseStatusCodePagesWithReExecute("~/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();