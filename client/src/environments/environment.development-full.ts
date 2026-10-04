import { AppConfig } from './environment.model';

export const environment: AppConfig = {
  kind: 'development',
  useMocks: false,
  authApi: 'http://localhost:5185/api/v1/auth',
  chatApi: 'http://localhost:5185/api/v1/chats',
  userApi: 'http://localhost:5185/api/v1/users',
  signalR: 'http://localhost:5185/hubs/v1/chat',
};
