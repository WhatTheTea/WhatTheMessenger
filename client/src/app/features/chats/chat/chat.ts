import { Component, inject, input } from '@angular/core';
import { ChatService } from '../../../core';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat {
  chatId = input.required<string>();
}
