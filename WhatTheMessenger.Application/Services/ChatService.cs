using Microsoft.EntityFrameworkCore;

using System;

using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Services;

public interface IChatService
{
    Task<Message> SendMessageAsync(NewMessageModel message, Chat chat, Guid userId);

    Task<Chat> CreateChatAsync(NewChatModel chat, User user);
}

public sealed class ChatService(IAppDbContext dbContext, IChatNotificationService notificationService) : IChatService
{
    public async Task<Chat> CreateChatAsync(NewChatModel newChat, User user)
    {
        var chat = user.CreateChat(newChat.Name);
        var otherParticipants = newChat.Participants.Except([user.Id]);

        if (newChat.Participants.Any(x => x != user.Id))
        {
            chat.Users = await dbContext.Users.TakeWhile(x => otherParticipants.Contains(x.Id)).ToListAsync();
        }
        await dbContext.SaveChangesAsync();
        await notificationService.NotifyChatCreated(chat);

        return chat;
    }

    public async Task<Message> SendMessageAsync(NewMessageModel model, Chat chat, Guid userId)
    {
        var message = chat.SendMessage(userId, model.Content);
        await dbContext.SaveChangesAsync();
        await notificationService.NotifyMessageSent(message);

        return message;
    }
}
