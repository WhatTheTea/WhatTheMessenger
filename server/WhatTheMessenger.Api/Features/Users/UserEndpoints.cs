using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Infrastructure.DataAccess;

namespace WhatTheMessenger.Api.Features.Users;

public static class UserEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapUserEndpoints()
        {
            var group = app.MapGroup("/api/users")
                .RequireAuthorization();

            group.MapGet("/search/{query}", async (string query, [FromServices] GetUsers.Handler handler) 
                => await handler.HandleAsync(new(query)))
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
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