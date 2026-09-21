using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api.Features.Users;

public record UserDto
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    
    // TODO: Remove
    public Guid[] ChatIds { get; set; } = [];

    public static UserDto From(User user) =>
        new()
        {
            DisplayName = user.DisplayName,
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            // ChatIds = user.Chats.Select(x => x.Id).ToArray()
        };

}