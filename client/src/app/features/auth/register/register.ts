import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm = new FormGroup(
    {
      displayName: new FormControl('', [Validators.required, Validators.maxLength(30)]),
      username: new FormControl('', [Validators.required, Validators.pattern('[a-zA-Z0-9_]*')]),
      password: new FormControl('', [Validators.required]),
      confirmPassword: new FormControl('', [Validators.required]),
    },
    [
      (control) => {
        const password = control.get('password')?.value;
        const confirm = control.get('confirmPassword')?.value;
        return password && confirm && password === confirm ? null : { passwordMismatch: true };
      },
    ],
  );

  register() {
    if (this.registerForm.invalid) return;

    const form = this.registerForm.value;

    this.authService
      .register({
        login: form.username ?? '',
        nickname: form.displayName ?? '',
        password: form.password ?? '',
      })
      .subscribe(() => {
        this.router.navigate([''], { queryParams: { register: null } });
      });
  }
}
