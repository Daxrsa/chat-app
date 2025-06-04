import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.baseApiUrl}/Auth`;

  constructor(private http: HttpClient) { }

  login(email: string, password: string, rememberMe: boolean): Observable<any> {
    const payload = {email, password, rememberMe};
    return this.http.post(`${this.apiUrl}/login-user`, payload);
  }

  register(email: string, password: string, firstName: string, lastName: string, username: string): Observable<any> {
    const payload = {email, password, firstName, lastName, username};
    return this.http.post(`${this.apiUrl}/register-user`, payload);
  }
}