import { Component, effect, inject, signal, viewChild } from '@angular/core';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';
import { ChatNavItem } from './chat-nav-item/chat-nav-item';
import { ChatService, UserService } from '../../core';
import { Chat } from '../../core/models/chat';
import { Chat as ChatComponent } from './chat/chat';
import { NbDialog } from '../../components/nb-dialog/nb-dialog';
import { NewChat } from './new-chat/new-chat';
import { guid } from '../../primitives';

@Component({
  selector: 'app-chats',
  imports: [ChatComponent, ChatNavItem, NbDialog, NewChat],
  templateUrl: './chat-list.html',
  styleUrl: './chat-list.scss',
})
export class ChatList {
  private authService = inject(AuthService);
  private router = inject(Router);
  private userService = inject(UserService);
  private chatService = inject(ChatService);

  userId = signal<guid>(this.authService.currentUser() ?? '');
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
    });
  }

  logout() {
    this.authService.logout().subscribe();
  }
}
