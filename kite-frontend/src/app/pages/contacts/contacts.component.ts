import { Component } from '@angular/core';
import { ContactsResponse } from '../../intefaces';
import { ContactsService } from '../../services/contacts.service';

@Component({
  selector: 'app-contacts',
  imports: [],
  templateUrl: './contacts.component.html',
  styleUrl: './contacts.component.css'
})
export class ContactsComponent {
  contacts: ContactsResponse[] = [];

  constructor(
    private contactService: ContactsService
  ) { }

  ngOnInit(): void {
    this.contactService.getContacts().subscribe({
      next: (data: ContactsResponse[]) => {
        this.contacts = data;
        console.log('Contacts received:', this.contacts);
      },
      error: (err) => {
        console.error('Error fetching contacts:', err);
      },
    });
  }
}
