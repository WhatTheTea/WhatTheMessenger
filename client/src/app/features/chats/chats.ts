import { Component, effect, inject, signal } from '@angular/core';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';
import { ChatNavItem } from './chat-nav-item/chat-nav-item';
import { ChatService, UserService } from '../../core';
import { Chat } from '../../core/models/chat';
import { Chat as ChatComponent } from './chat/chat';

@Component({
  selector: 'app-chats',
  imports: [ChatComponent, ChatNavItem],
  templateUrl: './chats.html',
  styleUrl: './chats.scss',
})
export class Chats {
  private authService = inject(AuthService);
  private router = inject(Router);
  private userService = inject(UserService);
  private chatService = inject(ChatService);

  chatId = signal<string | null>(null);
  userDisplayName = signal<string | null>(null);
  userChats = signal<Chat[]>([]);

  constructor() {
    effect(() => {
      if (!this.authService.isAuthenticated()) {
        this.router.navigate(['/']);
      }
    });

    this.userService.fetchUserInfo(this.authService.currentUser() ?? '').subscribe((user) => {
      this.userDisplayName.set(user.displayName ?? null);
    });

    this.chatService.getChatsForUser(this.authService.currentUser() ?? '').subscribe((chats) => {
      this.userChats.set(chats);
    })
  }

  logout() {
    this.authService.logout().subscribe();
  }
}
