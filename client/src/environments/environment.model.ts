export type EnvironmentKind = 'development' | 'staging' | 'production';

export interface Environment {
  kind: EnvironmentKind;
  useMocks: boolean;
}
