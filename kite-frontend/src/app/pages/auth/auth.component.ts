import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  imports: [CommonModule, FormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css',
  standalone: true
})
export class AuthComponent {
  isLogin = true;
  loading = false;
  errorMessage = '';
  showPassword = false;
  
  email = '';
  password = '';
  username = '';
  rememberMe = false;
  firstName = '';
  lastName = '';
  confirmPassword = '';
  termsAgreed = false;

  constructor(private authService: AuthService, private router: Router) {}

  switchtoRegister(): void {
    this.isLogin = false;
    this.resetFormFields();
  }

  switchToLogin(): void {
    this.isLogin = true;
    this.resetFormFields();
  }

  resetFormFields(): void {
    this.password = '';
    this.firstName = '';
    this.lastName = '';
    this.email = '';
    this.confirmPassword = '';
    this.rememberMe = false;
    this.termsAgreed = false;
    this.errorMessage = '';
  }

  handleSubmit(event: Event): void {
    event.preventDefault();
    
    if (this.isLogin) {
      this.login();
    } else {
      this.register();
    }
  }

  login(): void {
    this.loading = true;
    this.errorMessage = '';
    
    if (!this.email || !this.password) {
      this.errorMessage = 'Please enter both email and password';
      this.loading = false;
      return;
    }

    this.authService.login(this.email, this.password, this.rememberMe).subscribe({
      next: response => {
        localStorage.setItem('token', response.token);
        this.router.navigate(['/inbox']);
        this.loading = false;
        console.log('Login success:', response);
      },
      error: err => {
        // Handle error
        this.loading = false;
        this.errorMessage = err.error?.message || 'Login failed. Please try again.';
      }
    });
  }

  register(): void {
    this.loading = true;
    this.errorMessage = '';
    
    if (!this.firstName || !this.lastName || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all required fields';
      this.loading = false;
      return;
    }
    
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      this.loading = false;
      return;
    }
    
    if (!this.termsAgreed) {
      this.errorMessage = 'Please agree to the Terms of Service and Privacy Policy';
      this.loading = false;
      return;
    }

    this.authService.register(
      this.email,
      this.password,
      this.username,
      this.firstName,
      this.lastName,
    ).subscribe({
      next: response => {
        this.loading = false;
        console.log('Registration success:', response);
        this.switchToLogin(); 
      },
      error: err => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
      }
    });
  }
}