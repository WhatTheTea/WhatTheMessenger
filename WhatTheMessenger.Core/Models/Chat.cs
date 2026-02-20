namespace WhatTheMessenger.Core.Models;

public sealed class Chat
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public required List<User> Users { get; set; }
    public required List<Message> Messages { get; set; }
}
