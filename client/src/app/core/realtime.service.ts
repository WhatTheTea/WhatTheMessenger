import { Injectable } from '@angular/core';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../environments';

@Injectable()
export abstract class RealTimeService {
  public abstract start(): void;

  public abstract addMessageReceivedListener(handler: (message: any) => {}): void;
  public abstract addChatCreatedListener(handler: (chat: any) => {}): void;
}

export class SignalRService extends RealTimeService {
  private connection = new HubConnectionBuilder()
    .withUrl(environment.signalR)
    .withAutomaticReconnect()
    .build();

  start() {
    this.connection
      .start()
      .then(() => console.log('Connection started'))
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  addMessageReceivedListener(handler: (message: any) => {}) {
    this.connection.on('MessageReceived', handler);
  }

  addChatCreatedListener(handler: (chat: any) => {}) {
    this.connection.on('ChatCreated', handler);
  }
}

export class MockRealtimeService extends RealTimeService {
  override start(): void {}
  override addMessageReceivedListener(handler: (message: any) => {}): void {}
  override addChatCreatedListener(handler: (chat: any) => {}): void {}
}
