import { Injectable } from '@angular/core';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Chat, Message, MessageStatus } from '../chats';
import { environment } from '../../../environments';
import { from, fromEventPattern, Observable, of } from 'rxjs';

@Injectable()
export abstract class RealTimeService {
  public abstract start(): void;

  public abstract whenMessageReceived(): Observable<Message>;
  public abstract whenChatCreated(): Observable<Chat>;
}

@Injectable()
export class SignalRService extends RealTimeService {
  private static instance: SignalRService;

  constructor() {
    // Singleton pattern
    if (SignalRService.instance) {
      return SignalRService.instance;
    }
    super();
    SignalRService.instance = this;
  }

  private connection = new HubConnectionBuilder()
    .withUrl(environment.signalR, { withCredentials: true })
    .withAutomaticReconnect()
    .build();

  start() {
    this.connection
      .start()
      .then(() => console.log('Connection started'))
      .catch((err) => console.log('Error while starting connection: ' + err));
  }
  // TODO: Convert to observables

  whenMessageReceived(): Observable<Message> {
    return fromEventPattern(
      (h) => this.connection.on('MessageReceived', h),
      (h) => this.connection.off('MessageReceived', h),
    );
  }

  whenChatCreated(): Observable<Chat> {
    return fromEventPattern(
      (h) => this.connection.on('ChatCreated', h),
      (h) => this.connection.off('ChatCreated', h),
    );
  }

  addMessageReceivedListener(handler: (message: Message) => void) {
    this.connection.on('MessageReceived', handler);
  }

  addChatCreatedListener(handler: (chat: Chat) => void) {
    this.connection.on('ChatCreated', handler);
  }
}

@Injectable()
export class MockRealtimeService extends RealTimeService {
  public override whenMessageReceived(): Observable<Message> {
    return of({
        id: "test",
        chatId: "test",
        senderId: "test",
        senderName: "test",
        content: "test",
        sentAt: new Date(),
        status: MessageStatus.Read,
      });
  }

  public override whenChatCreated(): Observable<Chat> {
    return of({
        id: "test",
        name: "test",
        users: [],
        messages: [],
    });
  }

  override start(): void {}
}
