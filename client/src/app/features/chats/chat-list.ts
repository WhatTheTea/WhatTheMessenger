import { Component, effect, inject, input, signal } from '@angular/core';
import { AuthService } from '../../core/auth';
import { Router, RouterLinkActive, RouterLinkWithHref } from '@angular/router';
import { ChatService, RealTimeService, UserService } from '../../core';
import { Chat } from '../../core/models/chat';
import { Chat as ChatComponent } from './chat/chat';
import { NbDialog } from '../../components/nb-dialog/nb-dialog';
import { NewChat } from './new-chat/new-chat';
import { guid } from '../../primitives';
import { filter } from 'rxjs';

@Component({
  selector: 'app-chats',
  imports: [ChatComponent, NbDialog, NewChat, RouterLinkActive, RouterLinkWithHref],
  templateUrl: './chat-list.html',
  styleUrl: './chat-list.scss',
})
export class ChatList {
  private router = inject(Router);
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private chatService = inject(ChatService);
  private realtimeService = inject(RealTimeService);

  userId = signal<guid>(this.authService.currentUser() ?? '');
  id = input<guid>();
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

    this.realtimeService.addChatCreatedListener((chat) => {
      this.chatService
        .getChat(chat.chatId)
        .pipe(filter((x) => x != null))
        .subscribe((chat) => this.userChats.update((chats) => [...chats, chat]));
    });

    this.realtimeService.start();
  }

  logout() {
    this.authService.logout().subscribe();
  }
}
