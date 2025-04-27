import { Component } from '@angular/core';
import { LeftSidebarComponent } from '../../components/left-sidebar/left-sidebar.component';
import { RightSidebarComponent } from '../../components/right-sidebar/right-sidebar.component';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { ChatAreaComponent } from '../../components/chat-area/chat-area.component';

@Component({
  selector: 'app-chat-main',
  imports: [LeftSidebarComponent, RightSidebarComponent, NavbarComponent, ChatAreaComponent],
  templateUrl: './chat-main.component.html',
  styleUrl: './chat-main.component.css'
})
export class ChatMainComponent {

}
