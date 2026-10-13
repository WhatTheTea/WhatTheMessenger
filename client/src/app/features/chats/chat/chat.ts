import { Component, inject, input } from '@angular/core';
import { ChatService } from '..';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat {
  private chatService = inject(ChatService);

  chatId = input.required<string>();

  constructor(){
    this.chatService.
  }
}
