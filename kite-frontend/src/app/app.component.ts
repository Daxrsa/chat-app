import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatMainComponent } from './pages/chat-main/chat-main.component';
import { ContactsComponent } from './pages/contacts/contacts.component';
import { NavbarComponent } from './components/shared/navbar/navbar.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,NavbarComponent, ChatMainComponent, ContactsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'kite-frontend';
}
