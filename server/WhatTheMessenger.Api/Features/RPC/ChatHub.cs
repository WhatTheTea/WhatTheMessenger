using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WhatTheMessenger.Api.Features.Chats;

namespace WhatTheMessenger.Api.Features.RPC;

public interface IChatHub
{
    public Task MessageReceived(MessageDto message);

    public Task ChatCreated(ChatDto chat);
}

[Authorize]
public sealed class ChatHub : Hub<IChatHub>
{

}
