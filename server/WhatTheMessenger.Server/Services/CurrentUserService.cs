
using Microsoft.AspNetCore.Components.Authorization;

using WhatTheMessenger.Application.Services;

namespace WhatTheMessenger.Server.Services;

public sealed class CurrentUserService(AuthenticationStateProvider authenticationProvider) : ICurrentUserService
{
    public async Task<Guid?> GetUserId(CancellationToken cancellationToken = default)
    {
        var state = await authenticationProvider.GetAuthenticationStateAsync();

        if (!(state.User.Identity?.IsAuthenticated ?? false))
            return null;

        var userIdClaim = state.User.FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        if(!Guid.TryParse(userIdClaim!.Value, out var userId))
            return null;

        return userId;
    }
}
