import { Injectable } from '@angular/core';
import { forkJoin, map, Observable, of, tap } from 'rxjs';
import { Chat, CreateChat, Message } from '.';
import { guid } from '../../primitives';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments';
import { UserService } from '../users/user.service';

@Injectable()
export abstract class ChatService {
  abstract createChat(chat: CreateChat): Observable<void>;
  abstract getChatsForUser(): Observable<Chat[]>;
  abstract getChat(id: guid): Observable<Chat | null>;
}

@Injectable()
export class MockChatService extends ChatService {
  private chats: Map<guid, Chat> = new Map<guid, Chat>();

  constructor(private userService: UserService) {
    super();
  }

  override createChat(chat: CreateChat): Observable<void> {
    const chatId: guid = this.chats.size.toString();

    const users$ = chat.participants.map((x) => this.userService.fetchUserInfo(x));

    return forkJoin(users$).pipe(
      map((users) => {
        this.chats.set(chatId, {
          id: chatId,
          messages: [] as Message[],
          name: chat.name,
          users: users,
        });
      }),
    );
  }

  override getChatsForUser(): Observable<Chat[]> {
    return of(
      Array.from(this.chats.values()),
    );
  }

  override getChat(id: guid): Observable<Chat | null> {
    return of(this.chats.get(id) ?? null);
  }
}

@Injectable()
export class DatabaseChatService extends ChatService {
  constructor(private http: HttpClient) {
    super();
  }

  override createChat(chat: CreateChat): Observable<void> {
    return this.http.post<void>(environment.chatApi + '/create', chat, { withCredentials: true });
  }

  override getChatsForUser(): Observable<Chat[]> {
    return this.http.get<Chat[]>(environment.chatApi + '/user/me', {
      withCredentials: true,
    });
  }

  override getChat(id: guid): Observable<Chat | null> {
    return this.http.get<Chat | null>(environment.chatApi + '/user/me/' + id, {
      withCredentials: true,
    });
  }
}
