import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import { DataService } from '../../../infrastructure/services/data.service';
import { I18nService } from '../../../infrastructure/services/i18n.service';
import { LoadingService } from '../../../infrastructure/services/loading.service';
import { MATERIAL } from '../../../shared/material';

@Component({
  selector: 'app-posts-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    ...MATERIAL
  ],
  templateUrl: './posts-list.html',
  styleUrls: ['./posts-list.scss']
})
export class PostsListComponent implements OnInit {
  public readonly isLoading = signal(false);
  searchTerm = '';
  selectedUserId: number | null = null;
  readonly dataService = inject(DataService);
  readonly loadingService = inject(LoadingService);
  readonly i18n = inject(I18nService);

  // Computed signals for reactive data
  readonly posts = this.dataService.posts;
  readonly users = this.dataService.users;
  readonly selectedUser = this.dataService.selectedUser;
  readonly filteredPosts = computed(() => {
    let posts = this.dataService.filteredPosts();
    
    if (this.searchTerm) {
      const search = this.searchTerm.toLowerCase();
      posts = posts.filter(post => 
        post.title.toLowerCase().includes(search) ||
        post.body.toLowerCase().includes(search)
      );
    }
    
    return posts;
  });

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.isLoading.set(true);
    
    // Load posts and users in parallel
    Promise.all([
      firstValueFrom(this.dataService.getPosts()),
      firstValueFrom(this.dataService.getUsers())
    ]).then(() => {
      this.isLoading.set(false);
    }).catch(error => {
      console.error('Error loading data:', error);
      this.isLoading.set(false);
    });
  }

  onUserFilter(userId: number | null): void {
    this.selectedUserId = userId;
    this.dataService.setSelectedUser(userId);
  }

  onSearchChange(): void {
    // Reactive filtering is handled by computed signal
  }

  clearSearch(): void {
    this.searchTerm = '';
  }

  getUserName(userId: number): string {
    const user = this.users().find(u => u.id === userId);
    return user?.name || 'Unknown';
  }

  deletePost(postId: number): void {
    if (confirm('Are you sure you want to delete this post?')) {
      this.dataService.deletePost(postId).subscribe({
        next: () => {
          console.log('Post deleted successfully');
        },
        error: (error) => {
          console.error('Error deleting post:', error);
        }
      });
    }
  }
}
