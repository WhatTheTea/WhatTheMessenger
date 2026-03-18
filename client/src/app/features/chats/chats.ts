import { Component, effect, inject } from '@angular/core';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-chats',
  imports: [],
  templateUrl: './chats.html',
  styleUrl: './chats.scss',
})
export class Chats {
  private authService = inject(AuthService);
  private router = inject(Router);

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
