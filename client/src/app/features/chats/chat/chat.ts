import { Component, effect, ElementRef, inject, input, OnInit, signal, ViewChild } from '@angular/core';
import { ChatService, Message } from '..';
import { AuthService } from '../../auth';
import { guid } from '../../../primitives';
import { RealTimeService } from '../../rpc/realtime.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  imports: [ReactiveFormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat implements OnInit {
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef<HTMLDivElement>;
  
  private chatService = inject(ChatService);
  private authService = inject(AuthService);
  private realtimeService = inject(RealTimeService);

  userId = signal<guid>(this.authService.currentUser()?.id ?? '');
  chatId = input.required<string>();
  messages = signal<Message[]>([]); // TODO: scroll paging

  messageForm = new FormGroup({
    message: new FormControl('', { nonNullable: true }),
  });

  constructor() {
    this.realtimeService.whenMessageReceived().subscribe((x) => {
      this.messages.update((messages) => [...messages, x]);
    });

    effect(() => {
      const currentChatId = this.chatId();

      this.chatService
        .getMessagesForChat(currentChatId, 40, null)
        .subscribe((x) => this.messages.set(x.messages.filter((x) => x).reverse()));
    });

    effect(() => {
      // Track dependency: triggers every time the messages signal array changes
      this.messages(); 

      // Wait for Angular to complete rendering the new DOM nodes
      requestAnimationFrame(() => {
        this.scrollToBottom();
      });
    });
  }

  private scrollToBottom(): void {
    if (this.scrollContainer?.nativeElement) {
      const el = this.scrollContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    }
  }

  ngOnInit(): void {}

  onSendMessage(): void {
    const messageText = this.messageForm.value?.message;
    if (messageText?.trim()) {
      this.chatService
        .sendMessage({
          chatId: this.chatId(),
          content: messageText,
          senderId: this.userId(),
        })
        .subscribe(() => {
          this.messageForm.setValue({message: ''});
        });
    }
  }
}
