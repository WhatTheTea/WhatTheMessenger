import { HttpClient } from '@angular/common/http';
import { computed, Injectable, signal } from '@angular/core';
import { concatMap, map, Observable, tap } from 'rxjs';
import { LoginDTO } from '../models';

type guid = string;

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUser = signal<guid | null>(null);
  isAuthenticated = computed(() => this.currentUser() !== null);

  constructor(private http: HttpClient) {}

  login(dto: LoginDTO): Observable<void> {
    return this.http
      .post<void>('/api/auth/login', dto, { withCredentials: true })
      .pipe(concatMap(() => this.fetchCurrentUser().pipe(map((_) => {}))));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>('/api/auth/logout', {}, { withCredentials: true })
      .pipe(tap((_) => this.currentUser.set(null)));
  }

  fetchCurrentUser(): Observable<guid> {
    return this.http
      .get<guid>('/api/auth/me', {
        withCredentials: true,
      })
      .pipe(tap((user) => this.currentUser.set(user)));
  }
}
