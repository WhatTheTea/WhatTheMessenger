using System;
using System.Linq;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api.Features.Chats;

public sealed record NewMessageModel
{
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public required string Content { get; set; } = string.Empty;
}

public sealed record MessageDto
{
    public static Expression<Func<Message, MessageDto>> FromEntity => 
        message => new()
        {
            Content = message.Content,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SenderName = message.Sender.DisplayName
        };

    public required string Content { get; set; }
    public required Guid ChatId { get; set; }
    public required Guid SenderId { get; set; }
    public required string SenderName { get; set; }
}

public sealed record ChatDto
{
    public static Expression<Func<Chat, ChatDto>> FromEntity => 
        chat => new()
        {
            Name = chat.Name ?? string.Empty,
            // TODO: Check if it maps correctly to SQL
            Messages = chat.Messages.Select(x => new MessageDto()
            {
                ChatId = x.ChatId,
                Content = x.Content,
                SenderId = x.SenderId,
                SenderName = x.Sender.UserName ?? string.Empty
            }).ToList(),
            Users = chat.Users.Select(x => x.Id).ToList(),
            Id = chat.Id,
        };

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public List<MessageDto> Messages { get; set; } = [];

    public List<Guid> Users { get; set; } = [];

}
