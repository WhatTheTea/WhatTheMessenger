import { AppConfig } from "./environment.model";

export const config : AppConfig = {
    kind : "production",
    useMocks : false,
    authApi : "/api/auth",
    chatApi : "/api/chats",
    signalR : "/hubs/chat"
};