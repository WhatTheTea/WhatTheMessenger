import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  login() {
    const data = this.loginForm.value;

    if (this.loginForm.valid) {
      this.authService
        .login({
          login: data.username ?? '',
          password: data.password ?? '',
          rememberMe: true,
        })
        .subscribe();
    }
  }

  register() {
    this.router.navigate([''], { queryParams: { register: true } });
  }
}
