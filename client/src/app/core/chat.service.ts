import { Injectable } from '@angular/core';
import { CreateChat } from './models/createChat';
import { Observable } from 'rxjs';
import { Chat } from './models/chat';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments';

@Injectable()
export abstract class ChatService {
  abstract createChat(chat: CreateChat): Observable<void>;
  abstract getChat(chatId: string): Observable<Chat>;
}

export class DatabaseChatService extends ChatService {
  constructor(private http: HttpClient) {
    super();
  }

  override createChat(chat: CreateChat): Observable<void> {
    return this.http.post<void>(environment.chatApi + '/create', chat, { withCredentials: true });
  }

  override getChat(chatId: string): Observable<Chat> {
    throw new Error('Method not implemented.');
  }
}
