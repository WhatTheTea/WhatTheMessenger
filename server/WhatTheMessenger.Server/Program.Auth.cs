using Microsoft.AspNetCore.Identity;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Core.Models;
using WhatTheMessenger.Infrastructure.DataAccess;
using WhatTheMessenger.Server.Services;

namespace WhatTheMessenger.Server;

public static partial class Configuration
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder ConfigureIdentityAuth()
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

        public WebApplicationBuilder ConfigureBlazorAuth()
        {
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        context.Response.Redirect("auth/login");
                    }
                    return Task.CompletedTask;
                };
            });

            return builder;
        }
    }
}
