using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api.Features.RPC;

public interface IChatNotificationService
{
    Task NotifyMessageSent(Message message);

    Task NotifyChatCreated(Chat chat);    
}
