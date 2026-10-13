import { Component, effect, inject, input, OnInit, signal } from '@angular/core';
import { ChatService, Message } from '..';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat implements OnInit {
  private chatService = inject(ChatService);

  chatId = input.required<string>();
  messages = signal<Message[]>([]); // TODO: scroll paging

  constructor(){
     effect(() => {
      this.chatService.getMessagesForChat(this.chatId(), 40, null)
      .subscribe(x => this.messages.set(x.messages));
    })
  }

  ngOnInit(): void {
  }
}
