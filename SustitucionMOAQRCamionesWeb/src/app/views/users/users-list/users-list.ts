import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DataService } from '../../../infrastructure/services/data.service';
import { MATERIAL } from '../../../shared/material';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [
    CommonModule,
    ...MATERIAL
  ],
  templateUrl: './users-list.html',
  styleUrls: ['./users-list.scss']
})
export class UsersListComponent implements OnInit {
  public readonly isLoading = signal(false);

  constructor(protected readonly dataService: DataService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.isLoading.set(true);
    
    this.dataService.getUsers().subscribe({
      next: () => this.isLoading.set(false),
      error: (error) => {
        console.error('Error loading users:', error);
        this.isLoading.set(false);
      }
    });
  }

  selectUser(userId: number): void {
    this.dataService.setSelectedUser(userId);
    // You could navigate to posts here
  }
}
