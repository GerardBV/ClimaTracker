import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCard } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterLink } from '@angular/router';
import { AuthServiceService } from '../../services/auth-service.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, 
    MatTabsModule, 
    CommonModule, 
    MatError, 
    MatFormField, 
    MatLabel, 
    MatCard, 
    MatInput, 
    MatButtonModule, 
    RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  serverError = '';

  constructor(private formBuilder: FormBuilder, private authService: AuthServiceService) {
    this.loginForm = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  async login(): Promise<void> {
    this.serverError = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const { userName, password } = this.loginForm.getRawValue();

    try {
      await this.authService.login(userName, password);
    } catch (error: any) {
      this.serverError = error?.error?.error ?? 'La connexion a échoué.';
    }
  }
}