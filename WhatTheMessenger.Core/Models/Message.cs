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
    public required Guid Id { get; set; }
    public required User Sender { get; set; }
    public required User Recipient { get; set; }
    public required string Content { get; set; }
    public required DateTime SentAt { get; set; }
    public required MessageStatus Status { get; set; }
}
