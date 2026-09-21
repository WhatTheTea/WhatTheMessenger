using System.Text.Json;
using Microsoft.AspNetCore.ResponseCompression;
using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Infrastructure.Hubs;
using WhatTheMessenger.Infrastructure.Services;
using WhatTheMessenger.Server;
using WhatTheMessenger.Server.Endpoints;

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
builder.ConfigureIdentityAuth();

builder.Services.AddTransient<IChatNotificationService, SignalRChatNotificationService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserService, UserService>();

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

app.MapAuthEndpoints();
app.MapChatEndpoints();
app.MapUserEndpoints();

if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
