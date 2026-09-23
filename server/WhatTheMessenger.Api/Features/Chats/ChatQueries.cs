using Microsoft.EntityFrameworkCore;
using WhatTheMessenger.Core;

namespace WhatTheMessenger.Api.Features.Chats;

public static class ChatQueries
{
    extension(IQueryable<Chat> chats)
    {
        public IQueryable<Chat> GetChatsForUser(Guid userId)
        {
            return chats
                    .Include(x => x.Messages)
                    .Include(x => x.Users)
                    .Where(chat => chat.Users.Any(user => user.Id == userId));
        }

    }
}
