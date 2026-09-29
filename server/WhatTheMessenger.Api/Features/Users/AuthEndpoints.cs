using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api.Features.Users;

public static class AuthEndpoints
{
    public const string AuthEndpointPrefix = "/api/v1/auth";

    extension(WebApplication app)
    {
        public WebApplication MapAuthEndpoints()
        {
            var group = app.MapGroup(AuthEndpointPrefix);

            group.MapGet("/me", async (ClaimsPrincipal principal, UserManager<User> userManager) =>
                {
                    var user = await userManager.GetUserAsync(principal);
                    return user is not null 
                        ? Results.Ok(UserDto.From(user)) 
                        : Results.Unauthorized();
                })
                .RequireAuthorization()
                .Produces<UserDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/logout", (SignInManager<User> signInManager) => signInManager.SignOutAsync())
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized);;
            
            group.MapPost("/login", async (LoginModel login, SignInManager<User> signInManager) =>
            {
                var user = signInManager.UserManager.FindByNameAsync(login.Login);

                var result = await signInManager.PasswordSignInAsync(login.Login, login.Password, login.RememberMe, false);
                return result.Succeeded ? Results.Ok() : Results.BadRequest();
            }).Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

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