import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FriendsResponse } from '../intefaces';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FriendsService {
  private friendsUrl = 'assets/data/friends.json';

  constructor(private http: HttpClient) { }

  getFriends(): Observable<FriendsResponse[]> {
    return this.http.get<FriendsResponse[]>(this.friendsUrl);
  }
}
