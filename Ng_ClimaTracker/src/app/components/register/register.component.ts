import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { AuthServiceService } from '../../services/auth-service.service';

const matchingPasswordsValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value;
  const confirmPass = control.get('confirmPass')?.value;

  return password === confirmPass ? null : { notMatchPassword: true };
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCard,
    MatError,
    MatFormField,
    MatLabel,
    MatInput,
    RouterLink
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  regisForm: FormGroup;
  serverErrors: string[] = [];

  constructor(private formBuilder: FormBuilder, private authService: AuthServiceService) {
    this.regisForm = this.formBuilder.group({
      userName: ['', Validators.required],
      mail: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPass: ['', Validators.required]
    }, { validators: matchingPasswordsValidator });
  }

  async register(): Promise<void> {
    this.serverErrors = [];

    if (this.regisForm.invalid) {
      this.regisForm.markAllAsTouched();
      return;
    }

    const { userName, mail, password, confirmPass } = this.regisForm.getRawValue();

    try {
      await this.authService.register(userName, mail, password, confirmPass);
    } catch (error: any) {
      const message = error?.error?.error ?? error?.error?.message ?? 'L inscription a échoué.';
      this.serverErrors = Array.isArray(message) ? message : [message];
    }
  }
}
