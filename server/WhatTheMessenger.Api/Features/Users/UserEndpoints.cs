using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Users;

public static class UserEndpoints
{
    public const string Prefix = "/api/v1/users";

    public static IServiceCollection AddUserHandlers(this IServiceCollection services) =>
        services.AddTransient<IHandler<GetUsers.Request, GetUsers.Response>, GetUsers.Handler>();

    extension(WebApplication app)
    {
        public WebApplication MapUserEndpoints()
        {
            var group = app.MapGroup(Prefix)
                .RequireAuthorization();

            group.MapGet("/search/{query}", async (string query, IHandler<GetUsers.Request, GetUsers.Response> handler) 
                => await handler.HandleAsync(new(query)))
                .Produces<UserDto[]>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/{id:guid}", async (Guid id, IAppDbContext dbContext) =>
                {
                    var user = await dbContext.Users.AsNoTracking()
                        .SingleOrDefaultAsync(x => x.Id == id);

                    return user is not null 
                        ? Results.Ok(UserDto.From(user))
                        : Results.NotFound();
                })
                .Produces<UserDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized);


            return app;
        }
    }
}