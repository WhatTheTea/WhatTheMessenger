using Microsoft.AspNetCore.Identity;
using WhatTheMessenger.Infrastructure.DataAccess;
using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api;

public static partial class Configuration
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureCookieIdentityAuth()
        {
            builder.Services.AddIdentityCore<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();
            
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });

            return builder;
        }
    }
}
