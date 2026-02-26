using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Server;

public static partial class Configuration
{
    public static WebApplicationBuilder ConfigureDataAccess(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string not found.");
        builder.Services.AddDbContext<IAppDbContext, ApplicationDbContext>(options =>
        {
            if (builder.Configuration.GetValue<bool>("single-process"))
            {
                options.UseSqlite("Filename=dev.db");
            }
            else
            {
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
            }
        });

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
