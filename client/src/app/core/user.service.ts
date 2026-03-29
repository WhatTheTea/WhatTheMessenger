import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from './models';
import { guid } from '../primitives';

@Injectable()
export abstract class UserService {
  abstract fetchCurrentUser(userId: guid): Observable<User>;
  abstract findUsers(query: string): Observable<User[]>;
}
