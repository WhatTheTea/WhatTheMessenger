using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Tests.Utils;

public sealed class Disposable<T>(Action dispose, T value) : IDisposable
{
    public T Value => value;

    public void Dispose() => dispose();

    public static implicit operator T(Disposable<T> disposable) => disposable.Value;
}

public sealed class SqliteFixture : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly SqliteConnection sqlite;

    public IDisposable GetAppDbContext(out IAppDbContext db)
    {
        dbContext.Database.EnsureCreated();
        db = dbContext;
        return new Disposable<IAppDbContext>(
        () =>
        {
            dbContext.Database.EnsureDeleted();
        },
        dbContext
    );
    }

    public SqliteFixture()
    {
        sqlite = new("Filename=:memory:");
        sqlite.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlite(sqlite)
        .Options;

        dbContext = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        sqlite.Dispose();
    }
}
