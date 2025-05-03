import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ConversationResponse } from '../intefaces';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  private messagesUrl = 'assets/data/messages.json';

  constructor(private http: HttpClient) { }

  getMessages(): Observable<ConversationResponse[]> {
    return this.http.get<ConversationResponse[]>(this.messagesUrl);
  }
}
