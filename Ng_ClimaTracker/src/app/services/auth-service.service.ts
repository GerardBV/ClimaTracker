import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { lastValueFrom } from 'rxjs/internal/lastValueFrom';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {

  serverUrl = "https://localhost:7104/api/";

  serverErrors: string[] = [];

  constructor(public http: HttpClient, private router: Router) { }

  async login(userName: string, password: string){
    let LoginDTO = {
      Username : userName,
      Password : password
    }
    let x = await lastValueFrom(this.http.post<any>(this.serverUrl + 'Account/Login', LoginDTO));
    console.log(x);
    sessionStorage.setItem('token', x.token)
    sessionStorage.setItem('username', x.username)

    this.router.navigate(['/home']);
  }

  async register(userName: string, password: string, confPassword: string)
  {
      let RegisterDTO = {
      PasswordConfirm : confPassword,
      Password : password,
      Username : userName
    }
    let x = await lastValueFrom(this.http.post<any>(this.serverUrl + 'Account/Register', RegisterDTO));
    console.log(x);
    this.login(RegisterDTO.Username, RegisterDTO.Password);
  }
}
