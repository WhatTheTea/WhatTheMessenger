namespace WhatTheMessenger.Api.Features;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}

public struct Nothing
{
    public static Nothing Default = new();
}