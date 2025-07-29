import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.baseApiUrl}/Auth`;

  constructor(private http: HttpClient) { }

  getLoggedInUser(): Observable<any> {
    return this.http.get(`${this.apiUrl}/get-logged-in-user`);
  }
}
