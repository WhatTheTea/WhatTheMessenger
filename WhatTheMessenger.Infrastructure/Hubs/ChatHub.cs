using Microsoft.AspNetCore.SignalR;
using WhatTheMessenger.Application.Models;

namespace WhatTheMessenger.Infrastructure.Hubs;

public interface IChatHub
{
    public Task MessageReceived(SendMessageModel message);

    public Task ChatCreated(NewChatModel chat);
}

public sealed class ChatHub : Hub<IChatHub>
{

}
