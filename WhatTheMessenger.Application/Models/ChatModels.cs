using System.ComponentModel.DataAnnotations;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Models;

public sealed record NewMessageModel
{
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public required string Content { get; set; } = string.Empty;
}

public sealed record NewChatModel
{
    public required string? Name { get; set; }

    public required List<Guid> Participants { get; set; }
}

public sealed record MessageDto
{
    public required string Content { get; set; }
    public required Guid ChatId { get; set; }

    public static MessageDto From(NewMessageModel newMessage, Guid chatId) =>
        new()
        {
            Content = newMessage.Content,
            ChatId = chatId
        };

    public static MessageDto From(Message message) =>
        new()
        {
            Content = message.Content,
            ChatId = message.ChatId
        };
}

public sealed record ChatDto
{
    public required Guid ChatId { get; set; }

    public static ChatDto From(Chat chat) =>
        new()
        {
            ChatId = chat.Id,
        };
}
