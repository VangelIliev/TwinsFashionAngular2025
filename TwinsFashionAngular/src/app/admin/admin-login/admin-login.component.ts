import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AdminService, AdminLoginRequest } from '../../services/admin.service';

@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class AdminLoginComponent implements OnInit {
  credentials: AdminLoginRequest = {
    username: '',
    password: ''
  };
  
  loading = false;
  error = '';

  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Check if already authenticated
    this.adminService.checkAuth().subscribe({
      next: (authStatus) => {
        console.log('Auth status received:', authStatus);
        if (authStatus.isAuthenticated) {
          console.log('User is authenticated, navigating to dashboard');
          this.router.navigate(['/admin/dashboard']);
        } else {
          console.log('User is not authenticated');
        }
      },
      error: (error) => {
        console.error('Error checking auth status:', error);
      }
    });
  }

  onSubmit(event?: Event): void {
    if (event) {
      event.preventDefault();
    }
    
    if (!this.credentials.username || !this.credentials.password) {
      this.error = 'Моля въведете username и парола';
      return;
    }

    this.loading = true;
    this.error = '';

    this.adminService.login(this.credentials).subscribe({
      next: (response) => {
        console.log('Login response:', response);
        this.loading = false;
        console.log('Navigating to dashboard after successful login');
        this.router.navigate(['/admin/dashboard']);
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Грешка при влизане';
        console.error('Login error:', error);
      }
    });
  }
}
