namespace WhatTheMessenger.Api.Features.Messages;

public static class MessageEndpoints
{
    public const string Prefix = "/api/v1/messages";

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services) =>
        services
            .AddSendMessage();

    extension(WebApplication app)
    {
        public WebApplication MapMessageEndpoints()
        {
            var group = app.MapGroup(Prefix)
                .RequireAuthorization()
                .ProducesProblem(StatusCodes.Status401Unauthorized);

            group.MapPost("/send", async (SendMessage.Request request, IHandler<SendMessage.Request, SendMessage.Response> handler) =>
                {
                    var result = await handler.HandleAsync(request);

                    return result.Status switch
                    {
                        Core.MessageStatus.Sent => Results.Accepted(),
                        _ => Results.BadRequest()
                    };
                })
                .RequireAuthorization()
                .Accepts<SendMessage.Request>(System.Net.Mime.MediaTypeNames.Application.Json)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces<SendMessage.Response>(StatusCodes.Status202Accepted);

            return app;
        }
    }
}
