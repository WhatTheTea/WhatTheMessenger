using System.Security.Claims;

namespace WhatTheMessenger.Api.Features.Shared;

public static class ClaimsExtensions
{
    extension(ClaimsPrincipal claims)
    {
        public Guid GetUserId() => Guid.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    }
}
