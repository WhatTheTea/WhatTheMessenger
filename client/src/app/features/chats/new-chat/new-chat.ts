import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, filter, forkJoin, of, switchMap, tap } from 'rxjs';
import { guid } from '../../../primitives';
import { User } from '../../users/user';
import { UserService } from '../../users/user.service';
import { ChatService } from '../chat.service';

@Component({
  selector: 'app-new-chat',
  imports: [ReactiveFormsModule],
  templateUrl: './new-chat.html',
  styleUrl: './new-chat.scss',
})
export class NewChat implements OnInit {
  private userService = inject(UserService);
  private chatService = inject(ChatService);

  creatorId = input.required<guid>();
  created = output();

  // Group containing form controls
  newChatForm = new FormGroup({
    name: new FormControl('', { nonNullable: true }),
    query: new FormControl('', { nonNullable: true }),
  });

  searchResults = signal<User[]>([]);
  isLoading = signal<boolean>(false);
  selectedUsers = signal<User[]>([]);

  ngOnInit(): void {
    this.newChatForm.controls.query.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap((query) => {
          const trimmed = query.trim();
          if (!trimmed) {
            this.searchResults.set([]);
            this.isLoading.set(false);
          } else {
            this.isLoading.set(true);
          }
        }),
        filter((query) => query.trim().length > 0),
        switchMap((query) => this.userService.findUsers(query)),
      )
      .subscribe({
        next: (users) => {
          this.searchResults.set(users);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('User search failed:', err);
          this.searchResults.set([]);
          this.isLoading.set(false);
        },
      });
  }

  onFormSubmit(): void {
    const data = this.newChatForm.value;
    if (this.newChatForm.valid) {
      this.chatService
        .createChat({
          name: data.name ?? '',
          participants: [...this.selectedUsers().map((x) => x.id as guid), this.creatorId()],
        })
        .subscribe((_) => this.created.emit());
    }
  }

  toggleSelectUser(user: User): void {
    const current = this.selectedUsers();
    const exists = current.some((u) => u.id === user.id);
    if (exists) {
      this.selectedUsers.set(current.filter((u) => u.id !== user.id));
    } else {
      this.selectedUsers.set([...current, user]);
    }
  }

  isSelected(user: User): boolean {
    return this.selectedUsers().some((u) => u.id === user.id);
  }
}
