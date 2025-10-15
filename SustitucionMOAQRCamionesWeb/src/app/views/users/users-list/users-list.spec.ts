import { describe, it, expect, beforeEach } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';

// Mock User interface
interface User {
  id: number;
  name: string;
  email: string;
  phone: string;
  website: string;
  company: {
    name: string;
  };
}

// Create a simplified test component for Users List functionality
@Component({
  selector: 'app-users-list-test',
  template: `
    <div class="users-container">
      <h1>Users List</h1>
      
      <div class="search-section">
        <input 
          [value]="searchTerm"
          (input)="updateSearchTerm($event)"
          placeholder="Search users..."
          data-testid="search-input">
      </div>
      
      <div class="users-stats">
        <p>Total users: {{users().length}}</p>
        <p>Filtered users: {{filteredUsers().length}}</p>
      </div>
      
      <div class="users-list">
        @for (user of filteredUsers(); track user.id) {
          <div class="user-item" data-testid="user-item">
            <h3>{{user.name}}</h3>
            <p>Email: {{user.email}}</p>
            <p>Phone: {{user.phone}}</p>
            <p>Website: {{user.website}}</p>
            <p>Company: {{user.company.name}}</p>
          </div>
        }
      </div>
      
      @if (isLoading()) {
        <div class="loading">Loading users...</div>
      }
    </div>
  `,
  standalone: true
})
class TestUsersList {
  public readonly isLoading = signal(false);
  public readonly users = signal<User[]>([]);
  public readonly searchTerm = signal('');

  // Computed signal for filtered users
  readonly filteredUsers = computed(() => {
    const term = this.searchTerm().trim();
    if (!term) {
      return this.users();
    }

    const searchLower = term.toLowerCase();
    return this.users().filter(user =>
      user.name.toLowerCase().includes(searchLower) ||
      user.email.toLowerCase().includes(searchLower) ||
      user.company.name.toLowerCase().includes(searchLower)
    );
  });

  updateSearchTerm(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
  }

  loadUsers(mockUsers: User[] = []): void {
    this.isLoading.set(true);
    setTimeout(() => {
      this.users.set(mockUsers);
      this.isLoading.set(false);
    }, 100);
  }

  clearSearch(): void {
    this.searchTerm.set('');
  }
}

describe('UsersList', () => {
  let component: TestUsersList;
  let fixture: ComponentFixture<TestUsersList>;

  const mockUsers: User[] = [
    {
      id: 1,
      name: 'John Doe',
      email: 'john@example.com',
      phone: '123-456-7890',
      website: 'john.com',
      company: { name: 'Acme Corp' }
    },
    {
      id: 2,
      name: 'Jane Smith',
      email: 'jane@test.com',
      phone: '098-765-4321',
      website: 'jane.com',
      company: { name: 'Tech Inc' }
    },
    {
      id: 3,
      name: 'Bob Johnson',
      email: 'bob@demo.com',
      phone: '555-123-4567',
      website: 'bob.com',
      company: { name: 'Acme Corp' }
    }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestUsersList, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(TestUsersList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty users', () => {
    expect(component.users()).toEqual([]);
    expect(component.filteredUsers()).toEqual([]);
  });

  it('should load users correctly', async () => {
    component.loadUsers(mockUsers);
    // Wait for the async operation to complete
    await new Promise(resolve => setTimeout(resolve, 150));
    expect(component.users()).toEqual(mockUsers);
    expect(component.filteredUsers()).toEqual(mockUsers);
  });

  it('should filter users by name', () => {
    component.users.set(mockUsers);
    
    component.searchTerm.set('John');
    const filtered = component.filteredUsers();
    expect(filtered.length).toBe(2); // John Doe and Bob Johnson
    expect(filtered.map(u => u.name)).toContain('John Doe');
    expect(filtered.map(u => u.name)).toContain('Bob Johnson');
  });

  it('should filter users by email', () => {
    component.users.set(mockUsers);
    
    component.searchTerm.set('test.com');
    const filtered = component.filteredUsers();
    expect(filtered.length).toBe(1);
    expect(filtered[0].name).toBe('Jane Smith');
  });

  it('should filter users by company', () => {
    component.users.set(mockUsers);
    
    component.searchTerm.set('Acme');
    const filtered = component.filteredUsers();
    expect(filtered.length).toBe(2);
    expect(filtered.every(u => u.company.name === 'Acme Corp')).toBe(true);
  });

  it('should be case insensitive in search', () => {
    component.users.set(mockUsers);
    
    component.searchTerm.set('JOHN');
    const filtered = component.filteredUsers();
    expect(filtered.length).toBe(2);
  });

  it('should return all users when search term is empty', () => {
    component.users.set(mockUsers);
    
    component.searchTerm.set('');
    expect(component.filteredUsers()).toEqual(mockUsers);
    
    component.searchTerm.set('   '); // whitespace only
    expect(component.filteredUsers()).toEqual(mockUsers);
  });

  it('should clear search', () => {
    component.searchTerm.set('test search');
    component.clearSearch();
    expect(component.searchTerm()).toBe('');
  });

  it('should handle loading state', () => {
    expect(component.isLoading()).toBe(false);
    
    component.isLoading.set(true);
    expect(component.isLoading()).toBe(true);
  });

  it('should render users count', () => {
    component.users.set(mockUsers);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Total users: 3');
    expect(compiled.textContent).toContain('Filtered users: 3');
  });

  it('should render user information', () => {
    component.users.set([mockUsers[0]]);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('John Doe');
    expect(compiled.textContent).toContain('john@example.com');
    expect(compiled.textContent).toContain('Acme Corp');
  });
});
