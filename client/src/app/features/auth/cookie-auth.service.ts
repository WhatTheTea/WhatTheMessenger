import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { concatMap, filter, map, Observable, tap } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../../environments';
import { LoginDTO } from './login/login.dto';
import { User } from '../users/user';
import { RegisterDTO } from './register/register.dto';

@Injectable()
export class CookieAuthService extends AuthService {
  private _currentUser = signal<User | null>(null);
  currentUser = this._currentUser.asReadonly();

  constructor(private http: HttpClient) {
    super();
  }

  login(dto: LoginDTO): Observable<void> {
    return this.http
      .post<void>(`${environment.authApi}/login`, dto, { withCredentials: true })
      .pipe(concatMap(() => this.fetchCurrentUser().pipe(map(() => {}))));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${environment.authApi}/logout`, null, { withCredentials: true })
      .pipe(tap((_) => this._currentUser.set(null)));
  }

  fetchCurrentUser(): Observable<User> {
    return this.http
      .get<User>(`${environment.authApi}/me`, {
        withCredentials: true,
      })
      .pipe(
        filter((user) => !!user),
        tap((user) => this._currentUser.set(user))
      );
  }

  register(dto: RegisterDTO): Observable<void> {
    return this.http
      .post<void>(`${environment.authApi}/register`, dto)
      .pipe(concatMap(() => this.fetchCurrentUser().pipe(map(() => {}))));
  }
}
