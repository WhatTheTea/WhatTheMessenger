namespace WhatTheMessenger.Application;

public sealed record CreateChatCommand
{
    public required Guid[] ParticipantsIds { get; set; }
}

