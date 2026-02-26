using Microsoft.EntityFrameworkCore;

using System;

using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Services;

public interface IChatService
{
    Task<Message> SendMessageAsync(NewMessageModel message, Chat chat, Guid userId);

    Task<Chat> CreateChatAsync(NewChatModel chat);

    Task<Chat[]> GetChatsAsync(Guid userId);
}

public sealed class ChatService(IAppDbContext dbContext, IChatNotificationService notificationService) : IChatService
{
    public async Task<Chat> CreateChatAsync(NewChatModel newChat)
    {
        var users = await dbContext.Users.Where(x => newChat.Participants.Contains(x.Id)).ToListAsync();
        var chat = new Chat() 
        { 
            Name = newChat.Name ?? string.Join(", ", users.Select(x => x.DisplayName)), 
            Users = users 
        };

        dbContext.Chats.Add(chat);

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

    public Task<Chat[]> GetChatsAsync(Guid userId) =>
        dbContext.Chats.Include(x => x.Users)
            .Include(x => x.Messages)
            .Where(x => x.Users.Any(x => x.Id == userId))
            .ToArrayAsync();
}
