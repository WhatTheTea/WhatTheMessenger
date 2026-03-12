using System;

namespace WhatTheMessenger.Core.Models;

public sealed class Chat
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public required List<User> Users { get; set; }
    public List<Message> Messages { get; set; } = [];

    public bool HasUser(User user) => HasUser(user.Id);

    public bool HasUser(Guid userId) => Users.Any(u => u.Id == userId);

    public Message SendMessage(Guid senderId, string content)
    {
        if (!HasUser(senderId))
            throw new InvalidOperationException("User is not a member of the chat.");

        var message = new Message
        {
            Content = content,
            SentAt = DateTime.UtcNow,
            SenderId = senderId,
            Chat = this,
            Status = MessageStatus.Sent
        };

        Messages.Add(message);

        return message;
    }
}
