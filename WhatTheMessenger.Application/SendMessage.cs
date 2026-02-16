using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application;

public sealed record SendMessageCommand
{
    public required Guid ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
}
