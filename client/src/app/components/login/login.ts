import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})

export class Login {
  private authService = inject(AuthService)

  loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  login() {
    var data = this.loginForm.value

    if (this.loginForm.valid) {
      this.authService.login({
          login: data.username ?? "",
          password: data.password ?? "",
          rememberMe: true
        }).subscribe();
    }
  }
}
