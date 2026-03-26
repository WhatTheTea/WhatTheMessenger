import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { concatMap, map, Observable, tap } from 'rxjs';
import { LoginDTO, RegisterDTO } from '../models';
import { AuthService, guid } from './auth.service';

@Injectable()
export class CookieAuthService extends AuthService {
  private _currentUser = signal<guid | null>(null);
  currentUser = this._currentUser.asReadonly();

  constructor(private http: HttpClient) {
    super();
  }

  login(dto: LoginDTO): Observable<void> {
    return this.http
      .post<void>('/api/auth/login', dto, { withCredentials: true })
      .pipe(concatMap(() => this.fetchCurrentUser().pipe(map(() => {}))));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>('/api/auth/logout', null, { withCredentials: true })
      .pipe(tap((_) => this._currentUser.set(null)));
  }

  fetchCurrentUser(): Observable<guid> {
    return this.http
      .get<guid>('/api/auth/me', {
        withCredentials: true,
      })
      .pipe(tap((user) => this._currentUser.set(user)));
  }

  register(dto: RegisterDTO): Observable<void> {
    return this.http
      .post<void>('/api/auth/register', dto)
      .pipe(concatMap(() => this.fetchCurrentUser().pipe(map(() => {}))));
  }
}
