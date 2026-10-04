import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { guid } from '../../primitives';
import { User } from './user';
import { environment } from '../../../environments';

@Injectable()
export abstract class UserService {
  abstract fetchUserInfo(userId: guid): Observable<User>;
  abstract findUsers(query: string): Observable<User[]>;
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

  override findUsers(query: string): Observable<User[]> {
    return of(
      Array.from(this.users.values())
        .filter((x) => x.displayName.match(query) || x.username.match(query)),
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
  override findUsers(query: string): Observable<User[]> {
    return this.http
      .get<{ users: User[], count: number}>(`${environment.userApi}/search/${query}`, {
        withCredentials: true,
      })
      .pipe(map((x) => x?.users ?? []));
  }
}