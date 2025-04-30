import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

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
  
  // Form fields
  username = '';
  password = '';
  rememberMe = false;
  
  // Registration fields
  firstName = '';
  lastName = '';
  emailOrPhone = '';
  confirmPassword = '';
  termsAgreed = false;

  switchtoRegister(): void {
    this.isLogin = false;
    this.resetFormFields();
  }

  switchToLogin(): void {
    this.isLogin = true;
    this.resetFormFields();
  }

  resetFormFields(): void {
    this.username = '';
    this.password = '';
    this.firstName = '';
    this.lastName = '';
    this.emailOrPhone = '';
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
    
    // Validate inputs
    if (!this.username || !this.password) {
      this.errorMessage = 'Please enter both username and password';
      this.loading = false;
      return;
    }

    // Create login payload
    const loginData = {
      username: this.username,
      password: this.password,
      rememberMe: this.rememberMe
    };
    
    console.log('Login data:', loginData);
    
    // Simulate API call
    setTimeout(() => {
      this.loading = false;
      console.log('Login successful (simulation)');
      // Here you would normally navigate to the inbox
      // this.router.navigate(['/inbox']);
    }, 1500);
    
    // In real implementation you would:
    // this.authService.login(loginData).subscribe(
    //   response => { /* handle success */ },
    //   error => { /* handle error */ }
    // );
  }

  register(): void {
    this.loading = true;
    
    // Validate inputs
    if (!this.firstName || !this.lastName || !this.emailOrPhone || !this.username || !this.password) {
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

    // Create registration payload
    const registerData = {
      firstName: this.firstName,
      lastName: this.lastName,
      emailOrPhone: this.emailOrPhone,
      username: this.username,
      password: this.password
    };
    
    console.log('Registration data:', registerData);
    
    // Simulate API call
    setTimeout(() => {
      this.loading = false;
      console.log('Registration successful (simulation)');
      // Here you would normally navigate to the inbox or show a success message
    }, 1500);
    
    // In real implementation you would:
    // this.authService.register(registerData).subscribe(
    //   response => { /* handle success */ },
    //   error => { /* handle error */ }
    // );
  }
}
