using WhatTheMessenger.Core.Models;

namespace WhatTheMessenger.Application;

public sealed record ModifyChatParticipantsCommand
{
    public enum OperationType
    {
        Add,
        Remove
    }

    public required Guid ChatId { get; set; }
    public required Guid UserId { get; set; }
    public required OperationType Operation { get; set; }
}
