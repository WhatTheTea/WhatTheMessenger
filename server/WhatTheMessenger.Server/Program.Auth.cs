using Microsoft.AspNetCore.Identity;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Core.Models;
using WhatTheMessenger.Infrastructure.DataAccess;
using WhatTheMessenger.Server.Services;

namespace WhatTheMessenger.Server;

public static partial class Configuration
{
    public static WebApplicationBuilder ConfigureAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
        builder.Services.AddHttpContextAccessor();


        builder.Services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        return builder;
    }
}
