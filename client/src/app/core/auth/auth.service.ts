import { computed, Injectable, Signal } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginDTO, RegisterDTO } from '../models';

export type guid = string;

@Injectable()
export abstract class AuthService {
  public abstract currentUser : Signal<guid | null>
  public isAuthenticated = computed(() => this.currentUser() !== null);

  public abstract login(dto: LoginDTO): Observable<void>;
  public abstract logout(): Observable<void>;
  public abstract fetchCurrentUser(): Observable<guid | null>;
  public abstract register(dto: RegisterDTO): Observable<void>;
}