import { AppConfig } from './environment.model';

export const environment: AppConfig = {
  kind: 'development',
  useMocks: false,
  authApi: '/api/auth',
  chatApi: '/api/chats',
  userApi: '/api/users',
  signalR: '/hubs/chat',
};
