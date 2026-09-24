using System.Text.Json;
using Microsoft.AspNetCore.ResponseCompression;
using WhatTheMessenger.Infrastructure.Services;
using WhatTheMessenger.Api;
using WhatTheMessenger.Api.Features.Users;
using WhatTheMessenger.Api.Features.RPC;
using WhatTheMessenger.Api.Features.Chats;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    ["application/octet-stream"]);
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.ConfigureDataAccess();
builder.ConfigureCookieIdentityAuth();
builder.Services.AddAntiforgery();

builder.Services.AddTransient<IChatNotificationService, SignalRChatNotificationService>();

// Add slice handlers
builder.Services
    .AddUserHandlers()
    .AddChatHandlers()
    ;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    if (app.Configuration.GetValue<bool>("single-process"))
        app.EnsureDatabase();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseAntiforgery();

app
    .MapAuthEndpoints()
    .MapChatEndpoints()
    .MapUserEndpoints()
    ;

if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<ChatHub>("/hubs/v1/chat");

app.Run();
