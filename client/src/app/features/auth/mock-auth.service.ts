import { Observable, of } from 'rxjs';
import { AuthService} from './auth.service';
import { Injectable, signal } from '@angular/core';
import { guid } from '../../primitives';
import { LoginDTO } from './login/login.dto';
import { RegisterDTO } from './register/register.dto';
import { User } from '../users/user';

interface MockUser extends User {
  password: string;
}

@Injectable()
export class MockAuthService extends AuthService {
  private _users: Map<guid, MockUser> = new Map<guid, MockUser>();
  currentUser = signal<User | null>(null);

  login(dto: LoginDTO): Observable<void> {
    let user = this._users.get(dto.login);
    if (user?.password === dto.password) {
      this.currentUser.set(user);
    }

    return of(void 0);
  }

  logout(): Observable<void> {
    this.currentUser.set(null);

    return of(void 0);
  }

  fetchCurrentUser(): Observable<User | null> {
    return of(this.currentUser());
  }

  register(dto: RegisterDTO): Observable<void> {
    let user: MockUser = {
      id: dto.login,
      username: dto.login,
      displayName: dto.nickname,
      password: dto.password,
    };

    this._users.set(dto.login, user);

    this.currentUser.set(user);

    return of(void 0);
  }
}
