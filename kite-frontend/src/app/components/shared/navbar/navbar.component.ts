import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { User } from '../../../intefaces';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
  loggedInUser: User | null = null;
  loading = false;
  error: string = '';

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.loading = true;
    this.userService.getLoggedInUser().subscribe({
      next: (user: any) => {
        this.loggedInUser = user;
        this.loading = false;
        console.log("Received user:",user);
      },
      error: (err: any) => {
        this.error = err.error?.message || 'Failed to load user';
        this.loading = false;
        console.log("Error:", err);
      }
    });
  }

  removeToken(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/auth']);
  }
}
