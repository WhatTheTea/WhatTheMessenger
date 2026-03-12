using System.ComponentModel.DataAnnotations;

namespace WhatTheMessenger.Application.Models;

public sealed record RegisterModel
{
    [StringLength(100, MinimumLength = 1)]
    public required string Nickname { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public required string Login { get; set; }

    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}

public sealed record LoginModel
{
    [StringLength(100, MinimumLength = 1)]
    public required string Login { get; set; }

    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}
