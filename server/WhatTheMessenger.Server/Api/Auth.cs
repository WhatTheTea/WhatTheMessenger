using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Server.Api;

public static class Auth
{
    extension(WebApplication app)
    {
        public WebApplication MapAuthEndpoints()
        {
            var group = app.MapGroup("/api/auth");

            group.MapGet("/me", async (ClaimsPrincipal principal, UserManager<User> userManager) =>
                {
                    var user = await userManager.GetUserAsync(principal);
                    return user is not null 
                        ? Results.Ok(user.Id) 
                        : Results.Unauthorized();
                })
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/logout", (SignInManager<User> signInManager) => signInManager.SignOutAsync())
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);;
            
            group.MapPost("/login", async (LoginModel login, SignInManager<User> signInManager) =>
            {
                var result = await signInManager.PasswordSignInAsync(login.Login, login.Password, login.RememberMe, false);
                return result.Succeeded ? Results.Ok() : Results.Unauthorized();
            }).Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);;

            group.MapPost("/register", async (RegisterModel register, SignInManager<User> signInManager,
                IUserStore<User> userStore, UserManager<User> userManager) =>
            {
                var user = new User
                {
                    DisplayName = register.Nickname,
                };

                await userStore.SetUserNameAsync(user, register.Login.ToLower(), CancellationToken.None);
                var result = await userManager.CreateAsync(user, register.Password);

                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }
                
                await signInManager.SignInAsync(user, true);
                return Results.Ok();
            }).Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
            
            return app;
        }
    }
}