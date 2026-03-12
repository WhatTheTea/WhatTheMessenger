namespace WhatTheMessenger.Server.Api;

public static class Auth
{
    extension(WebApplication app)
    {
        public WebApplication MapJwtAuthEndpoints()
        {
            
            return app;
        }
    }
}