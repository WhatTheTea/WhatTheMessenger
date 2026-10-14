using System.Linq.Expressions;
using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api.Features.Messages;

public sealed record MessageDto
{
    public static Expression<Func<Message, MessageDto>> FromEntity =>
        message => new()
        {
            Id = message.Id,
            Content = message.Content,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SenderName = message.Sender.DisplayName
        };

    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public required Guid ChatId { get; set; }
    public required Guid SenderId { get; set; }
    public required string SenderName { get; set; }
}