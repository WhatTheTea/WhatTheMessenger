import { AppConfig } from "./environment.model";

export const environment : AppConfig = {
    kind : "production",
    useMocks : false,
    authApi : "/api/auth",
    chatApi : "/api/chats",
    signalR : "/hubs/chat"
};