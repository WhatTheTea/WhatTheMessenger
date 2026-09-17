import { Injectable } from '@angular/core';
import { CreateChat } from './models/createChat';
import { forkJoin, map, Observable, of, tap } from 'rxjs';
import { Chat } from './models/chat';
import { Message } from './models/message';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments';
import { guid } from '../primitives';
import { UserService } from './user.service';

@Injectable()
export abstract class ChatService {
  abstract createChat(chat: CreateChat): Observable<void>;
  abstract getChatsForUser(userId: string): Observable<Chat[]>;
}

@Injectable()
export class MockChatService extends ChatService {
  private chats: Map<guid, Chat> = new Map<guid, Chat>();

  constructor(private userService: UserService) {
    super();
  }

  override createChat(chat: CreateChat): Observable<void> {
    const chatId: guid = this.chats.size.toString();

    const users$ = chat.participantIds.map((x) => this.userService.fetchUserInfo(x));

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
  
  override getChatsForUser(userId: string): Observable<Chat[]> {
    return of(
      Array.from(this.chats.values()).filter((chat) =>
        chat.users.find((user) => user.id == userId),
      ),
    );
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

  override getChatsForUser(userId: string): Observable<Chat[]> {
    return this.http.get<Chat[]>(environment.chatApi + '/user/' + userId, {
      withCredentials: true,
    });
  }
}
