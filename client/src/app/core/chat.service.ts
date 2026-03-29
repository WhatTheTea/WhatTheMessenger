import { Injectable } from '@angular/core';
import { CreateChat } from './models/createChat';
import { Observable } from 'rxjs';

@Injectable()
export abstract class ChatService {
  abstract createChat(chat: CreateChat): Observable<void>;
}
