import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  serverUrl = "https://localhost:7104/api/";

  serverErrors: string[] = [];

  constructor(public http: HttpClient, private router: Router) { }

  async login(userName: string, password: string): Promise<void> {
    const LoginDTO = {
      Username : userName,
      Password : password
    }
    const response = await lastValueFrom(this.http.post<any>(this.serverUrl + 'Account/Login', LoginDTO));
    sessionStorage.setItem('token', response.token);
    sessionStorage.setItem('username', response.username);

    this.router.navigate(['/home']);
  }

  async register(userName: string, mail: string, password: string, confPassword: string): Promise<void> {
      const RegisterDTO = {
      PasswordConfirm : confPassword,
      Password : password,
      Username : userName,
      Email : mail
    }
    await lastValueFrom(this.http.post(this.serverUrl + 'Account/Register', RegisterDTO));
    //await this.login(RegisterDTO.Username, RegisterDTO.Password);
  }
}
