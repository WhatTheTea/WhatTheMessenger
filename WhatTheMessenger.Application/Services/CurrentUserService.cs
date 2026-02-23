namespace WhatTheMessenger.Application.Services
{
    public interface ICurrentUserService
    {
        Task<Guid?> GetUserId(CancellationToken cancellationToken = default);
    }
}
