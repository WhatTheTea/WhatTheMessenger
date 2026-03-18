import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
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

  register() {}
}
