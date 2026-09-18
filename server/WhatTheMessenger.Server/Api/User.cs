using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Application.Services;

namespace WhatTheMessenger.Server.Api;

public static class Users
{
    extension (WebApplication app)
    {
        public WebApplication MapUserEndpoints()
        {
            var group = app.MapGroup("/api/users");

            group.MapGet("/{id}", async (Guid id, IUserService userService) => 
                {
                    var result = userService.GetUserAsync(id);
                    
                    return result is not null ? Results.Ok(result)
                        : Results.NotFound();
                })
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/search/{query}", async (string query, IUserService userService) => userService.FindUsersAsync(query))
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            return app;
        }
    }
}