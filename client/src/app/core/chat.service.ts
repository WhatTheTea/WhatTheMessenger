import { Injectable } from '@angular/core';
import { CreateChat } from './models/createChat';
import { Observable } from 'rxjs';
import { Chat } from './models/chat';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments';

@Injectable()
export abstract class ChatService {
  abstract createChat(chat: CreateChat): Observable<void>;
  abstract getChatsForUser(chatId: string): Observable<Chat[]>;
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
    return this.http.get<Chat[]>(environment.chatApi + '/user/' + userId, { withCredentials: true });
  }
}
