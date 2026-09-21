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
    public required Guid SenderId { get; set; }
    public required string SenderName { get; set; }

    public static MessageDto From(Message message) =>
        new()
        {
            Content = message.Content,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SenderName = message.Sender.DisplayName
        };
}

public sealed record ChatDto
{
    public required Guid ChatId { get; set; }

    public required string Name { get; set; }

    public List<MessageDto> Messages { get; set; } = [];

    public List<Guid> Users { get; set; } = [];

    public static ChatDto From(Chat chat) =>
        new()
        {
            Name = chat.Name ?? string.Empty,
            Messages = chat.Messages.Select(MessageDto.From).ToList(),
            Users = chat.Users.Select(x => x.Id).ToList(),
            ChatId = chat.Id,
        };
}
