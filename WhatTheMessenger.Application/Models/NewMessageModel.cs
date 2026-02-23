using System.ComponentModel.DataAnnotations;

namespace WhatTheMessenger.Application.Models;

public sealed record NewMessageModel
{
    [StringLength(int.MaxValue, MinimumLength = 1)]
    public required string Content { get; set; } = string.Empty;
}
