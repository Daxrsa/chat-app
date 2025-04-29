import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Conversation } from '../intefaces';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  private messagesUrl = 'assets/data/messages.json';

  constructor(private http: HttpClient) { }

  getMessages(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(this.messagesUrl);
  }
}
