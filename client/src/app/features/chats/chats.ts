import { Component, effect, inject, signal } from '@angular/core';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';
import { Chat } from './chat/chat';
import { ChatNavItem } from './chat-nav-item/chat-nav-item';

@Component({
  selector: 'app-chats',
  imports: [Chat, ChatNavItem],
  templateUrl: './chats.html',
  styleUrl: './chats.scss',
})
export class Chats {
  private authService = inject(AuthService);
  private router = inject(Router);

  chatId = signal<string | null>(null);

  constructor() {
    effect(() => {
      if (!this.authService.isAuthenticated()) {
        this.router.navigate(['/']);
      }
    });
  }

  logout() {
    this.authService.logout().subscribe();
  }
}
