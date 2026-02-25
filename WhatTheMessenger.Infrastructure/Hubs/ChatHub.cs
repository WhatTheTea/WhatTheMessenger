using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WhatTheMessenger.Application.Models;

namespace WhatTheMessenger.Infrastructure.Hubs;

public interface IChatHub
{
    public Task MessageReceived(MessageDto message);

    public Task ChatCreated(ChatDto chat);
}

[Authorize]
public sealed class ChatHub : Hub<IChatHub>
{

}
