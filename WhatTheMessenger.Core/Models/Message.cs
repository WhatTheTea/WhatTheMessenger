namespace WhatTheMessenger.Core.Models;

public enum MessageStatus
{
    Fail,
    Sent,
    Delivered,
    Read
}

public sealed class Message
{
    public Guid Id { get; set; }
    public required Chat Chat { get; set; }
    public User Sender { get; set; } = null!;
    public Guid SenderId { get; set; }
    public required string Content { get; set; }
    public required DateTime SentAt { get; set; }
    public required MessageStatus Status { get; set; }
}
