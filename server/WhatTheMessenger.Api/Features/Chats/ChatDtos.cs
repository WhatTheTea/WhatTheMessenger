using System.Linq.Expressions;
using WhatTheMessenger.Core;
using WhatTheMessenger.Api.Features.Messages;

namespace WhatTheMessenger.Api.Features.Chats;

public sealed record ChatDto
{
    public static Expression<Func<Chat, ChatDto>> FromEntity =>
        chat => new()
        {
            Name = chat.Name ?? string.Empty,
            Messages = new List<MessageDto>()
            {
                chat.Messages.OrderByDescending(x => x.Id)
                    .Select(x => new MessageDto(){
                        Id = x.Id,
                        ChatId = x.ChatId,
                        Content = x.Content,
                        SenderId = x.SenderId,
                        SenderName = x.Sender.UserName ?? string.Empty
                    }).FirstOrDefault()!,
            },
            Users = chat.Users.Select(x => x.Id).ToList(),
            Id = chat.Id,
        };

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public List<MessageDto> Messages { get; set; } = [];

    public List<Guid> Users { get; set; } = [];

}
