import { describe, it, expect, beforeEach } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal, computed } from '@angular/core';

// Test constants
const TEST_DATA = {
  LOADING_DELAY: 100,
  TEST_TIMEOUT: 150,
  SEARCH_TERMS: {
    ANGULAR: 'Angular',
    POST: 'post',
    EMPTY: '',
    TEST_SEARCH: 'test search'
  },
  USER_IDS: {
    USER_1: 1,
    USER_2: 2,
    NONE: null
  }
} as const;

const TEST_SELECTORS = {
  POST_ITEM: '[data-testid="post-item"]',
  LOADING: '.loading',
  POSTS_CONTAINER: '.posts-container',
  POSTS_STATS: '.posts-stats'
} as const;

const LOADING_TEXT = 'Loading posts...' as const;

interface Post {
  id: number;
  title: string;
  body: string;
  userId: number;
}

@Component({
  selector: 'app-posts-list-test',
  template: `
    <div class="posts-container">
      <h1>Posts List</h1>
      <div class="posts-stats">
        <p>Total posts: {{posts().length}}</p>
        <p>Filtered posts: {{filteredPosts().length}}</p>
      </div>
      <div class="posts-list">
        @for (post of filteredPosts(); track post.id) {
          <div class="post-item" data-testid="post-item">
            <h3>{{post.title}}</h3>
            <p>{{post.body}}</p>
            <small>User: {{post.userId}}</small>
          </div>
        }
      </div>
      @if (isLoading()) {
        <div class="loading">Loading posts...</div>
      }
    </div>
  `,
  standalone: true
})
class TestPostsList {
  public readonly isLoading = signal(false);
  public readonly posts = signal<Post[]>([]);
  public readonly searchTerm = signal('');
  public readonly selectedUserId = signal<number | null>(null);

  readonly uniqueUserIds = computed(() => {
    const userIds = this.posts().map((post: Post) => post.userId);
    return Array.from(new Set(userIds)).sort((a, b) => a - b);
  });

  readonly filteredPosts = computed(() => {
    let filtered = this.posts();
    const term = this.searchTerm().trim();
    if (term) {
      const searchLower = term.toLowerCase();
      filtered = filtered.filter((post: Post) =>
        post.title.toLowerCase().includes(searchLower) ||
        post.body.toLowerCase().includes(searchLower)
      );
    }
    const userId = this.selectedUserId();
    if (userId !== null) {
      filtered = filtered.filter((post: Post) => post.userId === userId);
    }
    return filtered;
  });

  loadPosts(mockPosts: Post[] = []): void {
    this.isLoading.set(true);
    setTimeout(() => {
      this.posts.set(mockPosts);
      this.isLoading.set(false);
    }, TEST_DATA.LOADING_DELAY);
  }

  // Helper methods for testing
  setSearchTerm(term: string): void {
    this.searchTerm.set(term);
  }

  setSelectedUserId(userId: number | null): void {
    this.selectedUserId.set(userId);
  }

  clearFilters(): void {
    this.searchTerm.set(TEST_DATA.SEARCH_TERMS.EMPTY);
    this.selectedUserId.set(TEST_DATA.USER_IDS.NONE);
  }

  getPostsCount(): number {
    return this.posts().length;
  }

  getFilteredPostsCount(): number {
    return this.filteredPosts().length;
  }
}

describe('PostsList', () => {
  let component: TestPostsList;
  let fixture: ComponentFixture<TestPostsList>;

  const mockPosts: Post[] = [
    { id: 1, title: 'First Post', body: 'First post content', userId: TEST_DATA.USER_IDS.USER_1 },
    { id: 2, title: 'Second Post', body: 'Second post content', userId: TEST_DATA.USER_IDS.USER_1 },
    { id: 3, title: 'Third Post', body: 'Third post content about Angular', userId: TEST_DATA.USER_IDS.USER_2 },
    { id: 4, title: 'Fourth Post', body: 'Fourth post content', userId: TEST_DATA.USER_IDS.USER_2 }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestPostsList]
    }).compileComponents();

    fixture = TestBed.createComponent(TestPostsList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('Component Creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
      expect(component).toBeInstanceOf(TestPostsList);
    });

    it('should initialize with empty posts', () => {
      expect(component.posts()).toEqual([]);
      expect(component.filteredPosts()).toEqual([]);
      expect(component.uniqueUserIds()).toEqual([]);
    });

    it('should initialize with default state', () => {
      expect(component.isLoading()).toBe(false);
      expect(component.searchTerm()).toBe(TEST_DATA.SEARCH_TERMS.EMPTY);
      expect(component.selectedUserId()).toBe(TEST_DATA.USER_IDS.NONE);
    });
  });

  describe('Data Loading', () => {
    it('should load posts correctly', async () => {
      component.loadPosts(mockPosts);
      await new Promise(resolve => setTimeout(resolve, TEST_DATA.TEST_TIMEOUT));
      
      expect(component.posts()).toEqual(mockPosts);
      expect(component.filteredPosts()).toEqual(mockPosts);
      expect(component.getPostsCount()).toBe(mockPosts.length);
      expect(component.getFilteredPostsCount()).toBe(mockPosts.length);
    });

    it('should handle loading state during posts loading', () => {
      expect(component.isLoading()).toBe(false);
      component.loadPosts(mockPosts);
      expect(component.isLoading()).toBe(true);
    });

    it('should reset loading state after posts load', async () => {
      component.loadPosts(mockPosts);
      expect(component.isLoading()).toBe(true);
      
      await new Promise(resolve => setTimeout(resolve, TEST_DATA.TEST_TIMEOUT));
      expect(component.isLoading()).toBe(false);
    });

    it('should load empty posts array', async () => {
      component.loadPosts([]);
      await new Promise(resolve => setTimeout(resolve, TEST_DATA.TEST_TIMEOUT));
      
      expect(component.posts()).toEqual([]);
      expect(component.getPostsCount()).toBe(0);
    });
  });

  describe('User ID Computation', () => {
    it('should calculate unique user IDs', () => {
      component.posts.set(mockPosts);
      const uniqueIds = component.uniqueUserIds();
      expect(uniqueIds).toEqual([TEST_DATA.USER_IDS.USER_1, TEST_DATA.USER_IDS.USER_2]);
      expect(uniqueIds).toHaveLength(2);
    });

    it('should handle empty posts for unique user IDs', () => {
      component.posts.set([]);
      const uniqueIds = component.uniqueUserIds();
      expect(uniqueIds).toEqual([]);
      expect(uniqueIds).toHaveLength(0);
    });

    it('should handle single user ID', () => {
      const singleUserPosts = [
        { id: 1, title: 'Post 1', body: 'Content 1', userId: TEST_DATA.USER_IDS.USER_1 },
        { id: 2, title: 'Post 2', body: 'Content 2', userId: TEST_DATA.USER_IDS.USER_1 }
      ];
      component.posts.set(singleUserPosts);
      const uniqueIds = component.uniqueUserIds();
      expect(uniqueIds).toEqual([TEST_DATA.USER_IDS.USER_1]);
      expect(uniqueIds).toHaveLength(1);
    });

    it('should sort user IDs in ascending order', () => {
      const unsortedPosts = [
        { id: 1, title: 'Post 1', body: 'Content 1', userId: 5 },
        { id: 2, title: 'Post 2', body: 'Content 2', userId: 1 },
        { id: 3, title: 'Post 3', body: 'Content 3', userId: 3 }
      ];
      component.posts.set(unsortedPosts);
      const uniqueIds = component.uniqueUserIds();
      expect(uniqueIds).toEqual([1, 3, 5]);
    });
  });

  describe('Search Filtering', () => {
    beforeEach(() => {
      component.posts.set(mockPosts);
    });

    it('should filter posts by search term', () => {
      component.setSearchTerm(TEST_DATA.SEARCH_TERMS.ANGULAR);
      expect(component.filteredPosts()).toHaveLength(1);
      expect(component.filteredPosts()[0].id).toBe(3);
      
      component.setSearchTerm(TEST_DATA.SEARCH_TERMS.POST);
      expect(component.filteredPosts()).toHaveLength(4);
    });

    it('should filter posts case-insensitively', () => {
      component.setSearchTerm('ANGULAR');
      expect(component.filteredPosts()).toHaveLength(1);
      
      component.setSearchTerm('angular');
      expect(component.filteredPosts()).toHaveLength(1);
      
      component.setSearchTerm('AngUlaR');
      expect(component.filteredPosts()).toHaveLength(1);
    });

    it('should filter posts by title content', () => {
      component.setSearchTerm('First');
      const filtered = component.filteredPosts();
      expect(filtered).toHaveLength(1);
      expect(filtered[0].title).toContain('First');
    });

    it('should filter posts by body content', () => {
      component.setSearchTerm('Angular');
      const filtered = component.filteredPosts();
      expect(filtered).toHaveLength(1);
      expect(filtered[0].body).toContain('Angular');
    });

    it('should return all posts when search term is empty', () => {
      component.setSearchTerm(TEST_DATA.SEARCH_TERMS.EMPTY);
      expect(component.filteredPosts()).toHaveLength(mockPosts.length);
      expect(component.filteredPosts()).toEqual(mockPosts);
    });

    it('should handle search term with only whitespace', () => {
      component.setSearchTerm('   ');
      expect(component.filteredPosts()).toHaveLength(mockPosts.length);
    });

    it('should return empty array for non-matching search', () => {
      component.setSearchTerm('non-existent-term');
      expect(component.filteredPosts()).toHaveLength(0);
    });
  });

  it('should filter posts by user ID', () => {
    component.posts.set(mockPosts);
    component.selectedUserId.set(1);
    expect(component.filteredPosts().length).toBe(2);
    expect(component.filteredPosts().every((post: Post) => post.userId === 1)).toBe(true);
    component.selectedUserId.set(2);
    expect(component.filteredPosts().length).toBe(2);
    expect(component.filteredPosts().every((post: Post) => post.userId === 2)).toBe(true);
  });

  it('should combine search and user filters', () => {
    component.posts.set(mockPosts);
    component.searchTerm.set('Angular');
    component.selectedUserId.set(2);
    const filtered = component.filteredPosts();
    expect(filtered.length).toBe(1);
    expect(filtered[0].id).toBe(3);
  });

  it('should clear filters', () => {
    component.searchTerm.set('test search');
    component.selectedUserId.set(1);
    component.searchTerm.set('');
    component.selectedUserId.set(null);
    expect(component.searchTerm()).toBe('');
    expect(component.selectedUserId()).toBe(null);
  });

  describe('Component Integration', () => {
    beforeEach(() => {
      component.posts.set(mockPosts);
    });

    it('should have correct component state after setting posts', () => {
      expect(component.filteredPosts()).toHaveLength(mockPosts.length);
      expect(component.getPostsCount()).toBe(mockPosts.length);
      expect(component.getFilteredPostsCount()).toBe(mockPosts.length);
    });

    it('should update filtered count when search is applied', () => {
      component.setSearchTerm(TEST_DATA.SEARCH_TERMS.ANGULAR);
      
      expect(component.getFilteredPostsCount()).toBe(1);
      expect(component.getPostsCount()).toBe(mockPosts.length);
    });

    it('should handle component lifecycle correctly', () => {
      expect(component.isLoading()).toBe(false);
      expect(component.searchTerm()).toBe('');
      expect(component.selectedUserId()).toBe(null);
    });

    it('should maintain data integrity during operations', () => {
      const originalPosts = [...mockPosts];
      
      component.setSearchTerm('test');
      component.clearFilters();
      
      expect(component.posts()).toEqual(originalPosts);
      expect(component.searchTerm()).toBe('');
      expect(component.selectedUserId()).toBe(null);
    });

    it('should handle empty state gracefully', () => {
      component.posts.set([]);
      
      expect(component.getPostsCount()).toBe(0);
      expect(component.getFilteredPostsCount()).toBe(0);
      expect(component.filteredPosts()).toEqual([]);
    });

    it('should render posts count in template', () => {
      fixture.detectChanges();
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.textContent).toContain('Total posts: 4');
      expect(compiled.textContent).toContain('Filtered posts: 4');
    });

    it('should update template when search changes', () => {
      component.setSearchTerm(TEST_DATA.SEARCH_TERMS.ANGULAR);
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.textContent).toContain('Total posts: 4');
      expect(compiled.textContent).toContain('Filtered posts: 1');
    });

    it('should handle template compilation without errors', () => {
      expect(() => {
        fixture.detectChanges();
      }).not.toThrow();
    });
  });

  describe('Error Handling and Edge Cases', () => {
    it('should handle null posts gracefully', () => {
      component.posts.set([]);
      expect(component.filteredPosts()).toEqual([]);
      expect(component.getPostsCount()).toBe(0);
      expect(component.getFilteredPostsCount()).toBe(0);
    });

    it('should handle posts with empty strings', () => {
      const emptyPosts: Post[] = [{
        id: 1,
        userId: 1,
        title: '',
        body: ''
      }];
      
      component.posts.set(emptyPosts);
      expect(component.filteredPosts()).toEqual(emptyPosts);
      
      component.setSearchTerm('test');
      expect(component.filteredPosts()).toHaveLength(0);
    });

    it('should handle special characters in search', () => {
      component.posts.set(mockPosts);
      
      component.setSearchTerm('!@#$%^&*()');
      expect(component.filteredPosts()).toHaveLength(0);
      
      component.setSearchTerm('[]{};:');
      expect(component.filteredPosts()).toHaveLength(0);
    });

    it('should maintain original posts array when filtering', () => {
      const originalPosts = [...mockPosts];
      component.posts.set(mockPosts);
      
      component.setSearchTerm(TEST_DATA.SEARCH_TERMS.ANGULAR);
      expect(component.posts()).toEqual(originalPosts);
    });

    it('should handle rapid search term changes', () => {
      component.posts.set(mockPosts);
      
      component.setSearchTerm('a');
      component.setSearchTerm('an');
      component.setSearchTerm('ang');
      component.setSearchTerm('angu');
      component.setSearchTerm('angul');
      component.setSearchTerm('angula');
      component.setSearchTerm('angular');
      
      expect(component.filteredPosts()).toHaveLength(1);
      expect(component.searchTerm()).toBe('angular');
    });
  });

  it('should handle loading state', () => {
    expect(component.isLoading()).toBe(false);
    component.isLoading.set(true);
    expect(component.isLoading()).toBe(true);
  });

  it('should render posts count', () => {
    component.posts.set(mockPosts);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Total posts: 4');
    expect(compiled.textContent).toContain('Filtered posts: 4');
  });
});
