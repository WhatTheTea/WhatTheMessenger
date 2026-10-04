import { computed, Injectable, Signal } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginDTO } from './login/login.dto';
import { RegisterDTO } from './register/register.dto';
import { User } from '../users/user';

@Injectable()
export abstract class AuthService {
  public abstract currentUser: Signal<User | null>;
  public isAuthenticated = computed(() => this.currentUser() !== null);

  public abstract login(dto: LoginDTO): Observable<void>;
  public abstract logout(): Observable<void>;
  public abstract fetchCurrentUser(): Observable<User | null>;
  public abstract register(dto: RegisterDTO): Observable<void>;
}
