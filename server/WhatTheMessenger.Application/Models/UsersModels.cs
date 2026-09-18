namespace WhatTheMessenger.Application.Models;

public record UserModel
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public Guid[] ChatIds { get; set; } = [];
}