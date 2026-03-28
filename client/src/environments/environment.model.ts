export type EnvironmentKind = 'development' | 'staging' | 'production';

export interface AppConfig {
  kind: EnvironmentKind;
  useMocks: boolean;
  signalR: string;
  chatApi: string;
  authApi: string;
}
