import { describe, it, expect, beforeEach, vi } from 'vitest';
import { Injectable } from '@angular/core';
import { of, throwError } from 'rxjs';

// Test constants
const API_ENDPOINTS = {
  POSTS: 'https://jsonplaceholder.typicode.com/posts',
  USERS: 'https://jsonplaceholder.typicode.com/users'
} as const;

// Test data constants
const MOCK_DATA = {
  POST: { id: 1, title: 'Test Post', body: 'Test Body', userId: 1 },
  USER: { id: 1, name: 'Test User', email: 'test@test.com' }
} as const;

// Mock del HttpClient
const mockHttpClient = {
  get: vi.fn()
};

// Interface para typing mejor
interface HttpClientLike {
  get(url: string): any;
}

// Mock del DataService para testing básico
@Injectable()
class MockDataService {
  constructor(private readonly http: HttpClientLike) {}

  getPosts() {
    return this.http.get(API_ENDPOINTS.POSTS);
  }

  getUsers() {
    return this.http.get(API_ENDPOINTS.USERS);
  }
}

describe('DataService (Mock)', () => {
  let service: MockDataService;

  beforeEach(() => {
    vi.clearAllMocks();
    service = new MockDataService(mockHttpClient);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
    expect(service).toBeInstanceOf(MockDataService);
  });

  it('should call http.get for posts', async () => {
    const mockPosts = [MOCK_DATA.POST];
    mockHttpClient.get.mockReturnValue(of(mockPosts));

    const posts$ = service.getPosts();
    
    // Test synchronous subscription
    posts$.subscribe((posts: any) => {
      expect(posts).toEqual(mockPosts);
      expect(posts).toHaveLength(1);
      expect(posts[0]).toHaveProperty('id', MOCK_DATA.POST.id);
      expect(posts[0]).toHaveProperty('title', MOCK_DATA.POST.title);
    });

    expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.POSTS);
    expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
  });

  it('should call http.get for users', async () => {
    const mockUsers = [MOCK_DATA.USER];
    mockHttpClient.get.mockReturnValue(of(mockUsers));

    const users$ = service.getUsers();

    // Test synchronous subscription
    users$.subscribe((users: any) => {
      expect(users).toEqual(mockUsers);
      expect(users).toHaveLength(1);
      expect(users[0]).toHaveProperty('id', MOCK_DATA.USER.id);
      expect(users[0]).toHaveProperty('name', MOCK_DATA.USER.name);
      expect(users[0]).toHaveProperty('email', MOCK_DATA.USER.email);
    });

    expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.USERS);
    expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
  });

  describe('Error Handling', () => {
    it('should handle HTTP errors for posts', () => {
      const errorMessage = 'Network error';
      mockHttpClient.get.mockReturnValue(
        throwError(() => new Error(errorMessage))
      );

      const posts$ = service.getPosts();
      
      posts$.subscribe({
        error: (error: Error) => {
          expect(error.message).toBe(errorMessage);
        }
      });

      expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.POSTS);
      expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
    });

    it('should handle HTTP errors for users', () => {
      const errorMessage = 'Server error';
      mockHttpClient.get.mockReturnValue(
        throwError(() => new Error(errorMessage))
      );

      const users$ = service.getUsers();
      
      users$.subscribe({
        error: (error: Error) => {
          expect(error.message).toBe(errorMessage);
        }
      });

      expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.USERS);
      expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
    });
  });

  describe('Data Validation', () => {
    it('should handle empty posts array', () => {
      const emptyPosts: any[] = [];
      mockHttpClient.get.mockReturnValue(of(emptyPosts));

      service.getPosts().subscribe((posts: any) => {
        expect(posts).toEqual(emptyPosts);
        expect(posts).toHaveLength(0);
        expect(Array.isArray(posts)).toBe(true);
      });

      expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.POSTS);
      expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
    });

    it('should handle empty users array', () => {
      const emptyUsers: any[] = [];
      mockHttpClient.get.mockReturnValue(of(emptyUsers));

      service.getUsers().subscribe((users: any) => {
        expect(users).toEqual(emptyUsers);
        expect(users).toHaveLength(0);
        expect(Array.isArray(users)).toBe(true);
      });

      expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.USERS);
      expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
    });

    it('should handle multiple posts', () => {
      const multiplePosts = [
        MOCK_DATA.POST,
        { ...MOCK_DATA.POST, id: 2, title: 'Second Post' }
      ];
      mockHttpClient.get.mockReturnValue(of(multiplePosts));

      service.getPosts().subscribe((posts: any) => {
        expect(posts).toEqual(multiplePosts);
        expect(posts).toHaveLength(2);
        expect(posts[0].id).toBe(1);
        expect(posts[1].id).toBe(2);
      });

      expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.POSTS);
      expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
    });

    it('should handle multiple users', () => {
      const multipleUsers = [
        MOCK_DATA.USER,
        { ...MOCK_DATA.USER, id: 2, name: 'Second User', email: 'user2@test.com' }
      ];
      mockHttpClient.get.mockReturnValue(of(multipleUsers));

      service.getUsers().subscribe((users: any) => {
        expect(users).toEqual(multipleUsers);
        expect(users).toHaveLength(2);
        expect(users[0].id).toBe(1);
        expect(users[1].id).toBe(2);
      });

      expect(mockHttpClient.get).toHaveBeenCalledWith(API_ENDPOINTS.USERS);
      expect(mockHttpClient.get).toHaveBeenCalledTimes(1);
    });
  });
});
