import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from '../../core/auth';
import { Login } from '../../components/login/login';
import { Register } from '../../components/register/register';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Component({
  selector: 'app-home',
  imports: [Login, Register],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  authService = inject(AuthService);

  isRegister = toSignal(this.route.queryParamMap.pipe(map((params) => params.has('register'))), {
    initialValue: false,
  });

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigateByUrl("chats");
    }
  }
}
