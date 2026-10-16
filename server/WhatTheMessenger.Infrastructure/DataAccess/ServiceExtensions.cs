using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WhatTheMessenger.Infrastructure.DataAccess;

public static class ConfigurationExtensions
{
    public static DbContextOptionsBuilder ConfigureAppDbContext(this DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresConnection");

        if (configuration.GetValue<bool>("single-process"))
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

        return options;
    }
}
