import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { User } from './models';
import { guid } from '../primitives';
import { environment } from '../../environments';
import { HttpClient } from '@angular/common/http';

@Injectable()
export abstract class UserService {
  abstract fetchUserInfo(userId: guid): Observable<User>;
  abstract findUsers(query: string): Observable<guid[]>;
}

@Injectable()
export class MockUserService extends UserService {
  private users = new Map<guid, User>();

  override fetchUserInfo(userId: guid): Observable<User> {
    return new Observable((observer) => {
      let user = this.users.get(userId);
      if (!user) {
        user = {
          id: userId,
          displayName: `Mock User ${userId}`,
          username: `mock${userId}`,
          chatIds: [],
        } as User;
      }
      observer.next(user);
      observer.complete();
    });
  }

  override findUsers(query: string): Observable<guid[]> {
    return of(
      Array.from(this.users.values())
        .filter((x) => x.displayName.match(query) || x.username.match(query))
        .map((x) => x.id),
    );
  }
}

@Injectable()
export class DatabaseUserService extends UserService {
  constructor(private http: HttpClient) {
    super();
  }

  override fetchUserInfo(userId: guid): Observable<User> {
    return this.http.get<User>(`${environment.userApi}/${userId}`, {
      withCredentials: true,
    });
  }
  override findUsers(query: string): Observable<guid[]> {
    return this.http
      .get<guid[]>(`${environment.userApi}/search/${query}`, {
        withCredentials: true,
      })
      .pipe(map((x) => x ?? []));
  }
}
