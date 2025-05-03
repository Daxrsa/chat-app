import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ContactsResponse } from '../intefaces';

@Injectable({
  providedIn: 'root'
})
export class ContactsService {
  private contactsUrl = 'assets/data/contacts.json';

  constructor(private http: HttpClient) { }

  getContacts(): Observable<ContactsResponse[]> {
    return this.http.get<ContactsResponse[]>(this.contactsUrl);
  }
}
