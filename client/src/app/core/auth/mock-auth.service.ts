import { Observable, of } from 'rxjs';
import { LoginDTO, RegisterDTO } from '../models';
import { AuthService, guid } from './auth.service';
import { Injectable, signal } from '@angular/core';

interface _User {
  id: guid;
  name: string;
  nickname: string;
  password: string;
}

@Injectable()
export class MockAuthService extends AuthService {
  private _users: Map<guid, _User> = new Map<guid, _User>();
  currentUser = signal<guid | null>(null);

  login(dto: LoginDTO): Observable<void> {
    let user = this._users.get(dto.login);
    if (user?.password === dto.password) {
      this.currentUser.set(user.id);
    }

    return of(void 0);
  }

  logout(): Observable<void> {
    this.currentUser.set(null);

    return of(void 0);
  }

  fetchCurrentUser(): Observable<guid | null> {
    return of(this.currentUser());
  }

  register(dto: RegisterDTO): Observable<void> {
    let user: _User = {
      id: dto.login,
      name: dto.login,
      nickname: dto.nickname,
      password: dto.password,
    };

    this._users.set(dto.login, user);

    this.currentUser.set(user.id);

    return of(void 0);
  }
}
