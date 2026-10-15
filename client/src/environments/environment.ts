import { AppConfig } from './environment.model';

export const environment: AppConfig = {
  kind: 'production',
  useMocks: false,
  authApi: '/api/v1/auth',
  chatApi: '/api/v1/chats',
  userApi: '/api/v1/users',
  signalR: '/hubs/v1/chat',
  messagesApi: '/api/v1/messages'
};
