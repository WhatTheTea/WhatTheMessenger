import { guid } from '../../primitives';

export interface Message {
  id: guid;
  chatId: guid;
  senderId: guid;
  content: string;
  sentAt: Date;
  status: MessageStatus;
}

export interface IncomingMessage {
  chatId: guid;
  senderId: guid;
  content: string;
  senderName: string;
}

export enum MessageStatus {
  Fail,
  Sent,
  Delivered,
  Read,
}
