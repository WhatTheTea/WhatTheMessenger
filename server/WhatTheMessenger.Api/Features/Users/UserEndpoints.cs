using Microsoft.AspNetCore.Mvc;

namespace WhatTheMessenger.Api.Features.Users;

public static class UserEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapUserEndpoints()
        {
            var group = app.MapGroup("/api/users")
                .RequireAuthorization();

            group.MapGet("/{id:guid}", async (Guid id, IUserService userService) =>
                {
                    var result = await userService.GetUserAsync(id);

                    return result is not null ? Results.Ok(result)
                        : Results.NotFound();
                })
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/search", async ([FromBody] GetUsers.Request request, [FromServices] GetUsers.Handler handler, IUserService userService) 
                => await handler.HandleAsync(request))
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}