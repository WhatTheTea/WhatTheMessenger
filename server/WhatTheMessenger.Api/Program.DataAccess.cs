using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api;

public static partial class Configuration
{
    public static WebApplicationBuilder ConfigureDataAccess(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<IAppDbContext, ApplicationDbContext>(options =>
            options.ConfigureAppDbContext(builder.Configuration));

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        return builder;
    }

    public static WebApplication EnsureDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = (ApplicationDbContext)scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        dbContext.Database.EnsureCreated();

        return app;
    }
}
