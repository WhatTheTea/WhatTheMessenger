using Microsoft.AspNetCore.Identity;

namespace WhatTheMessenger.Core.Models;

public sealed class User : IdentityUser<Guid>
{
    public string DisplayName 
    {
        get => string.IsNullOrEmpty(field) ? UserName! : field;
        set;
    }

    public IEnumerable<Chat> Chats { get; set; } = [];
}
