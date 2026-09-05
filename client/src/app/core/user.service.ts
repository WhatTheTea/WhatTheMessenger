import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from './models';
import { guid } from '../primitives';
import { environment } from '../../environments';
import { HttpClient } from '@angular/common/http';

@Injectable()
export abstract class UserService {
  abstract fetchUserInfo(userId: guid): Observable<User>;
  abstract findUsers(query: string): Observable<User[]>;
}

@Injectable()
export class MockUserService extends UserService {
  override fetchUserInfo(userId: guid): Observable<User> {
    return new Observable(observer => {
      observer.next({
        id: userId,
        displayName: 'Mock User',
        username: 'mockuser',
        chatIds: [],
      } as User);
      observer.complete();
    });
  }
  override findUsers(query: string): Observable<User[]> {
    throw new Error('Method not implemented.');
  }
  
}

@Injectable()
export class DatabaseUserService extends UserService {
  constructor(private http: HttpClient) 
  {
    super();
  }

  override fetchUserInfo(userId: guid): Observable<User> {
    return this.http
          .get<User>(`${environment.authApi}/me`, {
            withCredentials: true,
          })
  }
  override findUsers(query: string): Observable<User[]> {
    throw new Error('Method not implemented.');
  }

}