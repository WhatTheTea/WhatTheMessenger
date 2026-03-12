using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application.Interfaces;

public interface IChatNotificationService
{
    Task NotifyMessageSent(Message message);

    Task NotifyChatCreated(Chat chat);    
}
