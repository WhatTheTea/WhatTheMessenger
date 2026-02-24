using Microsoft.AspNetCore.SignalR;
using WhatTheMessenger.Application.Models;

namespace WhatTheMessenger.Infrastructure.Hubs;

public interface IChatHub
{
    public Task MessageReceived(MessageDto message);

    public Task ChatCreated(ChatDto chat);
}

public sealed class ChatHub : Hub<IChatHub>
{

}
