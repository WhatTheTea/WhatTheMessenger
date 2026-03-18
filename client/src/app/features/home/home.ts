import { Component, inject, input } from '@angular/core';
import { ActivatedRoute } from '@angular/router'

import { AuthService } from '../../core/auth'
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

export class Home {
  private route = inject(ActivatedRoute);
  authService = inject(AuthService);
  
  isRegister = toSignal(
    this.route.queryParamMap.pipe(
      map(params => params.has('register'))
    ),
    { initialValue: false }
  );
}
