import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatMainComponent } from './pages/chat-main/chat-main.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ChatMainComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'kite-frontend';
}
