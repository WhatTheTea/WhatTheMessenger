using Microsoft.EntityFrameworkCore;

using System;

using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Services;

public interface IChatService
{
    Task<Message> SendMessage(NewMessageModel message, Chat chat, Guid userId);
}

public sealed class ChatService(IAppDbContext dbContext) : IChatService
{
    public async Task<Message> SendMessage(NewMessageModel model, Chat chat, Guid userId)
    {
        var message = chat.SendMessage(userId, model.Content);
        await dbContext.SaveChangesAsync();
        return message;
    }
}
