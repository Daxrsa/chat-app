import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-left-sidebar',
  imports: [FormsModule],
  templateUrl: './left-sidebar.component.html',
  styleUrl: './left-sidebar.component.css'
})
export class LeftSidebarComponent {
  isNewMode = true;

  onToggleChange(): void {
    console.log('Current mode:', this.isNewMode ? 'New' : 'Unread');
  }
}