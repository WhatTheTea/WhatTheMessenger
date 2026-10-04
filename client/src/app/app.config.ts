import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  Provider,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { catchError, of } from 'rxjs';

import { AuthService, CookieAuthService, MockAuthService } from './features/auth';
import { routes } from './app.routes';
import { environment } from '../environments';
import { ChatService, MockChatService, DatabaseChatService } from './features/chats';
import { RealTimeService, MockRealtimeService, SignalRService } from './features/rpc/realtime.service';
import { UserService, MockUserService, DatabaseUserService } from './features/users/user.service';

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
