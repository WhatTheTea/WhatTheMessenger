using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

using WhatTheMessenger.Application.Interfaces;
using WhatTheMessenger.Application.Services;
using WhatTheMessenger.Infrastructure.Hubs;
using WhatTheMessenger.Infrastructure.Services;
using WhatTheMessenger.Server;
using WhatTheMessenger.Server.App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    ["application/octet-stream"]);
});

builder.ConfigureDataAccess();
builder.ConfigureAuth();

builder.Services.AddTransient<IChatNotificationService, SignalRChatNotificationService>();
builder.Services.AddScoped<IChatService, ChatService>();

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
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseResponseCompression();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
