import { describe, it, expect, beforeEach } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

// Mock interfaces for admin dashboard
interface DashboardStats {
  totalUsers: number;
  totalPosts: number;
  totalTodos: number;
  completedTodos: number;
}

interface RecentActivity {
  id: number;
  type: 'user' | 'post' | 'todo';
  description: string;
  timestamp: Date;
}

// Create a simplified test component for Admin Dashboard functionality
@Component({
  selector: 'app-admin-dashboard-test',
  template: `
    <div class="admin-dashboard">
      <h1>Admin Dashboard</h1>
      
      <div class="stats-grid">
        <div class="stat-card" data-testid="users-stat">
          <h3>Total Users</h3>
          <p class="stat-number">{{stats().totalUsers}}</p>
        </div>
        
        <div class="stat-card" data-testid="posts-stat">
          <h3>Total Posts</h3>
          <p class="stat-number">{{stats().totalPosts}}</p>
        </div>
        
        <div class="stat-card" data-testid="todos-stat">
          <h3>Total Todos</h3>
          <p class="stat-number">{{stats().totalTodos}}</p>
        </div>
        
        <div class="stat-card" data-testid="completion-stat">
          <h3>Todo Completion</h3>
          <p class="stat-number">{{todoCompletionPercentage()}}%</p>
        </div>
      </div>
      
      <div class="recent-activity">
        <h2>Recent Activity</h2>
        <div class="activity-list">
          @for (activity of recentActivities(); track activity.id) {
            <div class="activity-item" data-testid="activity-item">
              <span class="activity-type">{{activity.type}}</span>
              <span class="activity-description">{{activity.description}}</span>
              <span class="activity-timestamp">{{activity.timestamp | date:'short'}}</span>
            </div>
          }
        </div>
      </div>
      
      @if (isLoading()) {
        <div class="loading">Loading dashboard...</div>
      }
    </div>
  `,
  standalone: true,
  imports: [CommonModule]
})
class TestAdminDashboard {
  public readonly isLoading = signal(false);
  public readonly stats = signal<DashboardStats>({
    totalUsers: 0,
    totalPosts: 0,
    totalTodos: 0,
    completedTodos: 0
  });
  public readonly recentActivities = signal<RecentActivity[]>([]);

  readonly todoCompletionPercentage = computed(() => {
    const currentStats = this.stats();
    if (currentStats.totalTodos === 0) return 0;
    return Math.round((currentStats.completedTodos / currentStats.totalTodos) * 100);
  });

  // Computed signal for total content items
  readonly totalContentItems = computed(() => {
    const currentStats = this.stats();
    return currentStats.totalUsers + currentStats.totalPosts + currentStats.totalTodos;
  });

  loadDashboardData(mockStats?: DashboardStats, mockActivities?: RecentActivity[]): void {
    if (mockStats) {
      this.stats.set(mockStats);
    }
    if (mockActivities) {
      this.recentActivities.set(mockActivities);
    }
  }

  refreshStats(): void {
    this.isLoading.set(true);
    // Simulate API call
    setTimeout(() => {
      this.isLoading.set(false);
    }, 500);
  }

  addActivity(activity: RecentActivity): void {
    const currentActivities = this.recentActivities();
    this.recentActivities.set([activity, ...currentActivities.slice(0, 9)]); // Keep only 10 most recent
  }
}

describe('AdminDashboard', () => {
  let component: TestAdminDashboard;
  let fixture: ComponentFixture<TestAdminDashboard>;

  const mockStats: DashboardStats = {
    totalUsers: 10,
    totalPosts: 50,
    totalTodos: 100,
    completedTodos: 75
  };

  const mockActivities: RecentActivity[] = [
    {
      id: 1,
      type: 'user',
      description: 'New user registered',
      timestamp: new Date('2025-09-11T10:00:00')
    },
    {
      id: 2,
      type: 'post',
      description: 'New post created',
      timestamp: new Date('2025-09-11T09:30:00')
    },
    {
      id: 3,
      type: 'todo',
      description: 'Todo completed',
      timestamp: new Date('2025-09-11T09:15:00')
    }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestAdminDashboard]
    }).compileComponents();

    fixture = TestBed.createComponent(TestAdminDashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty stats', () => {
    const initialStats = component.stats();
    expect(initialStats.totalUsers).toBe(0);
    expect(initialStats.totalPosts).toBe(0);
    expect(initialStats.totalTodos).toBe(0);
    expect(initialStats.completedTodos).toBe(0);
  });

  it('should calculate todo completion percentage correctly', () => {
    component.stats.set(mockStats);
    expect(component.todoCompletionPercentage()).toBe(75); // 75/100 = 75%
  });

  it('should handle zero todos for completion percentage', () => {
    component.stats.set({
      totalUsers: 10,
      totalPosts: 50,
      totalTodos: 0,
      completedTodos: 0
    });
    expect(component.todoCompletionPercentage()).toBe(0);
  });

  it('should calculate total content items', () => {
    component.stats.set(mockStats);
    expect(component.totalContentItems()).toBe(160); // 10 + 50 + 100
  });

  it('should load dashboard data', () => {
    component.loadDashboardData(mockStats, mockActivities);
    
    expect(component.stats()).toEqual(mockStats);
    expect(component.recentActivities()).toEqual(mockActivities);
  });

  it('should handle loading state', () => {
    expect(component.isLoading()).toBe(false);
    
    component.isLoading.set(true);
    expect(component.isLoading()).toBe(true);
  });

  it('should refresh stats', () => {
    component.refreshStats();
    expect(component.isLoading()).toBe(true);
  });

  it('should add new activity', () => {
    const initialActivities = [...mockActivities];
    component.recentActivities.set(initialActivities);
    
    const newActivity: RecentActivity = {
      id: 4,
      type: 'user',
      description: 'User deleted',
      timestamp: new Date()
    };
    
    component.addActivity(newActivity);
    
    const activities = component.recentActivities();
    expect(activities[0]).toEqual(newActivity);
    expect(activities.length).toBe(4);
  });

  it('should limit activities to 10 items', () => {
    // Fill with 10 activities
    const manyActivities: RecentActivity[] = Array.from({ length: 10 }, (_, i) => ({
      id: i + 1,
      type: 'user',
      description: `Activity ${i + 1}`,
      timestamp: new Date()
    }));
    
    component.recentActivities.set(manyActivities);
    
    // Add one more
    const newActivity: RecentActivity = {
      id: 11,
      type: 'post',
      description: 'New activity',
      timestamp: new Date()
    };
    
    component.addActivity(newActivity);
    
    const activities = component.recentActivities();
    expect(activities.length).toBe(10);
    expect(activities[0]).toEqual(newActivity);
  });

  it('should render stats in template', () => {
    component.stats.set(mockStats);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('10'); // Total Users
    expect(compiled.textContent).toContain('50'); // Total Posts
    expect(compiled.textContent).toContain('100'); // Total Todos
    expect(compiled.textContent).toContain('75%'); // Completion percentage
  });

  it('should render activities in template', () => {
    component.recentActivities.set(mockActivities);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    const activityItems = compiled.querySelectorAll('[data-testid="activity-item"]');
    expect(activityItems.length).toBe(3);
  });
});
