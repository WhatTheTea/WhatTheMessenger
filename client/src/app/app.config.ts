import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  Provider,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { catchError, of } from 'rxjs';

import { AuthService, CookieAuthService, MockAuthService } from './core/auth';
import { ChatService, DatabaseChatService, DatabaseUserService, MockChatService, MockRealtimeService, MockUserService, RealTimeService, SignalRService, UserService } from './core';
import { routes } from './app.routes';
import { environment } from '../environments';

function provideServices(): Provider[] {
  if (environment.useMocks) {
    return [
      { provide: AuthService, useClass: MockAuthService },
      { provide: RealTimeService, useClass: MockRealtimeService },
      { provide: UserService, useClass: MockUserService },
      { provide: ChatService, useClass: MockChatService },
    ];
  } else {
    return [
      { provide: AuthService, useClass: CookieAuthService },
      { provide: RealTimeService, useClass: SignalRService },
      { provide: UserService, useClass: DatabaseUserService },  
      { provide: ChatService, useClass: DatabaseChatService },
    ];
  }
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),
    provideServices(),
    provideAppInitializer(() => {
      const auth = inject(AuthService);
      return auth.fetchCurrentUser().pipe(
        catchError((err) => {
          console.debug(err);
          return of(null);
        }),
      );
    }),
  ],
};
