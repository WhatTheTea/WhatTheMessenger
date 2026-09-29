using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Tests.Utils;

public sealed class Disposable(Action dispose) : IDisposable
{
    public void Dispose() => dispose();
}

public class DbFixture : IDisposable
{
    private readonly SqliteConnection sqlite;

    public ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(sqlite)
            .Options;
        var appDbContext = new ApplicationDbContext(options);

        return appDbContext;
    }

    public IDisposable UseDb()
    {
        using var context = GetDbContext();
        context.Database.EnsureCreated();

        return new Disposable(() =>
        {
            using var context = GetDbContext();
            context.Database.EnsureDeleted();
            context.Dispose();
        });
    }

    public DbFixture()
    {
        sqlite = new("Filename=:memory:");
        sqlite.Open();
    }

    public void Dispose()
    {
        sqlite.Dispose();
    }
}
