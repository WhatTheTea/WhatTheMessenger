import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { catchError, of } from 'rxjs';

import { routes } from './app.routes';
import { AuthService, CookieAuthService, MockAuthService } from './core/auth';
import { environment } from '../environments';
import { MockRealtimeService, RealTimeService, SignalRService } from './core/realtime';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    
    environment.useMocks
      ? { provide: AuthService, useClass: MockAuthService }
      : { provide: AuthService, useClass: CookieAuthService },
    
    environment.useMocks
      ? { provide: RealTimeService, useClass: MockRealtimeService}
      : { provide: RealTimeService, useClass: SignalRService },

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
