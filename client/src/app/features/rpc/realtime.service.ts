import { Injectable } from '@angular/core';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { Chat, IncomingChat, IncomingMessage } from '../chats';
import { environment } from '../../../environments';

@Injectable()
export abstract class RealTimeService {
  public abstract start(): void;

  public abstract addMessageReceivedListener(handler: (message: IncomingMessage) => void): void;
  public abstract addChatCreatedListener(handler: (chat: Chat) => void): void;
}

@Injectable()
export class SignalRService extends RealTimeService {
  private connection = new HubConnectionBuilder()
    .withUrl(environment.signalR, {withCredentials: true})
    .withAutomaticReconnect()
    .build();

  start() {
    this.connection
      .start()
      .then(() => console.log('Connection started'))
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  addMessageReceivedListener(handler: (message: IncomingMessage) => void) {
    this.connection.on('MessageReceived', handler);
  }

  addChatCreatedListener(handler: (chat: Chat) => void) {
    this.connection.on('ChatCreated', handler);
  }
}

@Injectable()
export class MockRealtimeService extends RealTimeService {
  override start(): void {}
  override addMessageReceivedListener(handler: (message: IncomingMessage) => void): void {}
  override addChatCreatedListener(handler: (chat: Chat) => void): void {}
}
