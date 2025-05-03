import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FriendsResponse } from '../../intefaces';
import { FriendsService } from '../../services/friends.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-left-sidebar',
  imports: [FormsModule, CommonModule],
  templateUrl: './left-sidebar.component.html',
  styleUrl: './left-sidebar.component.css'
})
export class LeftSidebarComponent {
  isNewMode = true;

  friends: FriendsResponse[] = [];

  constructor(
    private friendService: FriendsService
  ) { }

  ngOnInit(): void {
    this.friendService.getFriends().subscribe({
      next: (data: FriendsResponse[]) => {
        this.friends = data;
        console.log('Friends received:', this.friends);
      },
      error: (err) => {
        console.error('Error fetching friends:', err);
      },
    });
  }

  onToggleChange(): void {
    console.log('Current mode:', this.isNewMode ? 'New' : 'Unread');
  }
}