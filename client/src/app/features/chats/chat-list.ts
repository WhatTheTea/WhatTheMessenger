import { Component, effect, inject, input, signal } from '@angular/core';
import { AuthService } from '../auth';
import { Router, RouterLinkActive, RouterLinkWithHref } from '@angular/router';
import { Chat } from './chat';
import { Chat as ChatComponent } from './chat/chat';
import { NbDialog } from '../../components/nb-dialog/nb-dialog';
import { NewChat } from './new-chat/new-chat';
import { guid } from '../../primitives';
import { filter } from 'rxjs';
import { RealTimeService } from '../rpc/realtime.service';
import { UserService } from '../users/user.service';
import { ChatService } from './chat.service';

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

  userId = signal<guid>(this.authService.currentUser()?.id ?? '');
  id = input<guid>();
  userDisplayName = signal<string | null>(null);
  userChats = signal<Chat[]>([]);

  constructor() {
    this.chatService.getChatsForUser().subscribe((chats) => {
      this.userChats.set(chats);
    });

    this.realtimeService.addChatCreatedListener((chat) => {
      this.chatService
        .getChat(chat.id)
        .pipe(filter((x) => x != null))
        .subscribe((chat) => { 
          this.userChats.update((chats) => [...chats, chat]);
          this.router.navigate(['chats', chat.id]);
        });
    });

    effect(() => {
      if (!this.authService.isAuthenticated()) {
        this.router.navigate(['/']);
      }
    });

    this.userService.fetchUserInfo(this.authService.currentUser()?.id ?? '').subscribe((user) => {
      this.userDisplayName.set(user.displayName ?? null);
      this.realtimeService.start();
    });

  }

  logout() {
    this.authService.logout().subscribe();
  }
}
