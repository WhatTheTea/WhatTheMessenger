/// Using own handler interface is meant to prevent the indirection caused by Mediator-like frameworks 

namespace WhatTheMessenger.Api.Features;

public interface IHandler<in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}

public struct Nothing
{
    public static Nothing Default = new();
}