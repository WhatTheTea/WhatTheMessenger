using Microsoft.AspNetCore.Identity;

namespace WhatTheMessenger.Core.Models;

public sealed class User : IdentityUser<Guid>
{
    private List<Chat> chats = [];

    public string DisplayName 
    {
        get => string.IsNullOrEmpty(field) ? UserName! : field;
        set;
    }

    public IReadOnlyCollection<Chat> Chats { get => chats.AsReadOnly(); }

    public Chat CreateChat(string? name = null)
    {
        var chat = new Chat
        {
            Name = name,
            Users = [this]
        };

        chats.Add(chat);

        return chat;
    }
}
