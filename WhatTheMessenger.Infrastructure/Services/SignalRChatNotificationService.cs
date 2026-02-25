using Microsoft.AspNetCore.SignalR;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Models;
using WhatTheMessenger.Core.Models;
using WhatTheMessenger.Infrastructure.Hubs;

namespace WhatTheMessenger.Infrastructure.Services;
// TODO: Wire it up and add Redis pub-sub
public class SignalRChatNotificationService(IHubContext<ChatHub, IChatHub> hub) : IChatNotificationService
{
    public Task NotifyChatCreated(Chat chat)
    {
        var model = ChatDto.From(chat);
        var receiverIds = IdsFrom(chat.Users);

        return hub.Clients.Users(receiverIds).ChatCreated(model);
    }

    public Task NotifyMessageSent(Message message)
    {
        var model = MessageDto.From(message);
        var receiverIds = IdsFrom(message.Chat.Users);

        return hub.Clients.Users(receiverIds).MessageReceived(model);
    }

    private static IReadOnlyList<string> IdsFrom(IEnumerable<User> users) => 
        users.Select(x => x.Id)
            .Select(x => x.ToString())
            .ToList()
            .AsReadOnly();
}
