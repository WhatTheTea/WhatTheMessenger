import { Injectable } from '@angular/core';
import { forkJoin, map, Observable, of, tap } from 'rxjs';
import { Chat, CreateChat, Message } from '.';
import { guid } from '../../primitives';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments';
import { UserService } from '../users/user.service';

interface Messages {
  messages : Message[],
  count: number,
  lastMessageId: guid | null
}

@Injectable()
export abstract class ChatService {
  abstract createChat(chat: CreateChat): Observable<void>;
  abstract getChatsForUser(): Observable<Chat[]>;
  abstract getChat(id: guid): Observable<Chat | null>;
  abstract getMessagesForChat(id: guid, count : number, lastMessageId: guid | null): Observable<Messages> 
}

@Injectable()
export class MockChatService extends ChatService {
  private chats: Map<guid, Chat> = new Map<guid, Chat>();
  
  constructor(private userService: UserService) {
    super();
  }

  override getMessagesForChat(id: guid, count: number, lastMessageId: guid | null): Observable<Messages>  {
    const messages = this.chats.get(id)?.messages;
    return of({ messages: messages, count: messages?.length, lastMessageId: null } as Messages);
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

  override getMessagesForChat(id: guid, count: number | null, lastMessageId: guid | null): Observable<Messages>  {
    return this.http.get<Messages>(
      environment.chatApi 
      + `/${id}/` 
      + `?count=${count ?? 20}` 
      + (lastMessageId ? `&lastMessageId=${lastMessageId}` : ''), {withCredentials: true});
  }

  override createChat(chat: CreateChat): Observable<void> {
    return this.http.post<void>(environment.chatApi + '/create', chat, { withCredentials: true });
  }

  override getChatsForUser(): Observable<Chat[]> {
    return this.http.get<Chat[]>(environment.chatApi, {
      withCredentials: true,
    });
  }

  override getChat(id: guid): Observable<Chat | null> {
    return this.http.get<Chat | null>(environment.chatApi + '/' + id, {
      withCredentials: true,
    });
  }
}
