import { Component, inject } from '@angular/core';

import { AuthService } from '../../core/auth'
import { Login } from '../../components/login/login';

@Component({
  selector: 'app-home',
  imports: [Login],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})

export class Home {
  authService = inject(AuthService);
  
}
