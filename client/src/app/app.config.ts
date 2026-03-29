import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  Provider,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { catchError, of } from 'rxjs';

import { AuthService, CookieAuthService, MockAuthService } from './core/auth';
import { MockRealtimeService, RealTimeService, SignalRService } from './core/realtime';
import { routes } from './app.routes';
import { environment } from '../environments';

function provideServices(): Provider[] {
  if (environment.useMocks) {
    return [
      { provide: AuthService, useClass: MockAuthService },
      { provide: RealTimeService, useClass: MockRealtimeService },
    ];
  } else {
    return [
      { provide: AuthService, useClass: CookieAuthService },
      { provide: RealTimeService, useClass: SignalRService },
    ];
  }
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
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
