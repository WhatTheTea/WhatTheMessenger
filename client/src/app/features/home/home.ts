import { Component, effect, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from '../auth';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { Login } from '../auth/login/login';
import { Register } from '../auth/register/register';

@Component({
  selector: 'app-home',
  imports: [Login, Register],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  authService = inject(AuthService);

  isRegister = toSignal(this.route.queryParamMap.pipe(map((params) => params.has('register'))), {
    initialValue: false,
  });

  constructor() {
    effect(() => {
      if (this.authService.isAuthenticated()) {
        this.router.navigateByUrl('chats');
      }
    });
  }
}
