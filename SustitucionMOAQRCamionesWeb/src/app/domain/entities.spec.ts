import { describe, it, expect, beforeEach } from 'vitest';

// Define interfaces and types that would typically be in entities.ts
interface BaseEntity {
  id: number;
  createdAt: Date;
  updatedAt: Date;
}

interface User extends BaseEntity {
  name: string;
  email: string;
  active: boolean;
  role: UserRole;
  profile?: UserProfile;
}

interface UserProfile {
  firstName: string;
  lastName: string;
  avatar?: string;
  bio?: string;
  phone?: string;
  address?: Address;
}

interface Address {
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

enum UserRole {
  ADMIN = 'admin',
  USER = 'user',
  MODERATOR = 'moderator',
  GUEST = 'guest'
}

interface Post extends BaseEntity {
  title: string;
  content: string;
  published: boolean;
  authorId: number;
  author?: User;
  tags: string[];
  views: number;
  likes: number;
  comments?: Comment[];
}

interface Comment extends BaseEntity {
  content: string;
  postId: number;
  authorId: number;
  author?: User;
  parentId?: number; // For nested comments
  replies?: Comment[];
}

interface Todo extends BaseEntity {
  title: string;
  description?: string;
  completed: boolean;
  dueDate?: Date;
  priority: TodoPriority;
  userId: number;
  user?: User;
  categoryId?: number;
  category?: TodoCategory;
}

enum TodoPriority {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  URGENT = 'urgent'
}

interface TodoCategory extends BaseEntity {
  name: string;
  color: string;
  description?: string;
  userId: number;
}

// API Response types
interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
  timestamp: Date;
}

interface PaginatedResponse<T> extends ApiResponse<T[]> {
  pagination: {
    page: number;
    limit: number;
    total: number;
    totalPages: number;
    hasNext: boolean;
    hasPrev: boolean;
  };
}

interface ErrorResponse {
  error: {
    code: string;
    message: string;
    details?: any;
  };
  success: false;
  timestamp: Date;
}

// Utility functions for entities
class EntityUtils {
  static createBaseEntity(id?: number): BaseEntity {
    const now = new Date();
    return {
      id: id || Date.now(),
      createdAt: now,
      updatedAt: now
    };
  }

  static updateTimestamp<T extends BaseEntity>(entity: T): T {
    return {
      ...entity,
      updatedAt: new Date()
    };
  }

  static isEntityValid<T extends BaseEntity>(entity: T): boolean {
    return !!(entity.id && entity.createdAt && entity.updatedAt);
  }

  static sortByCreatedDate<T extends BaseEntity>(entities: T[], ascending = true): T[] {
    return [...entities].sort((a, b) => {
      const comparison = a.createdAt.getTime() - b.createdAt.getTime();
      return ascending ? comparison : -comparison;
    });
  }

  static filterByDateRange<T extends BaseEntity>(
    entities: T[], 
    startDate: Date, 
    endDate: Date
  ): T[] {
    return entities.filter(entity => 
      entity.createdAt >= startDate && entity.createdAt <= endDate
    );
  }

  static groupByProperty<T extends Record<string, any>>(
    entities: T[], 
    property: keyof T
  ): Record<string, T[]> {
    return entities.reduce((groups, entity) => {
      const key = String(entity[property]);
      if (!groups[key]) {
        groups[key] = [];
      }
      groups[key].push(entity);
      return groups;
    }, {} as Record<string, T[]>);
  }
}

// Entity builders/factories
class UserFactory {
  static create(data: Partial<User>): User {
    return {
      ...EntityUtils.createBaseEntity(),
      name: data.name || 'Unknown User',
      email: data.email || 'user@example.com',
      active: data.active ?? true,
      role: data.role || UserRole.USER,
      profile: data.profile
    };
  }

  static createWithProfile(userData: Partial<User>, profileData: Partial<UserProfile>): User {
    const user = UserFactory.create(userData);
    user.profile = {
      firstName: profileData.firstName || '',
      lastName: profileData.lastName || '',
      avatar: profileData.avatar,
      bio: profileData.bio,
      phone: profileData.phone,
      address: profileData.address
    };
    return user;
  }
}

class PostFactory {
  static create(data: Partial<Post>): Post {
    const baseEntity = EntityUtils.createBaseEntity();
    return {
      ...baseEntity,
      ...data,
      title: data.title || 'Untitled Post',
      content: data.content || '',
      published: data.published ?? false,
      authorId: data.authorId || 1,
      author: data.author,
      tags: data.tags || [],
      views: data.views || 0,
      likes: data.likes || 0,
      comments: data.comments
    };
  }
}

class TodoFactory {
  static create(data: Partial<Todo>): Todo {
    return {
      ...EntityUtils.createBaseEntity(),
      title: data.title || 'New Todo',
      description: data.description,
      completed: data.completed ?? false,
      dueDate: data.dueDate,
      priority: data.priority || TodoPriority.MEDIUM,
      userId: data.userId || 1,
      user: data.user,
      categoryId: data.categoryId,
      category: data.category
    };
  }
}

describe('Entities', () => {
  describe('BaseEntity', () => {
    it('should create a valid base entity', () => {
      const entity = EntityUtils.createBaseEntity();
      
      expect(entity.id).toBeDefined();
      expect(entity.createdAt).toBeInstanceOf(Date);
      expect(entity.updatedAt).toBeInstanceOf(Date);
      expect(entity.createdAt.getTime()).toBeLessThanOrEqual(entity.updatedAt.getTime());
    });

    it('should create base entity with specific id', () => {
      const customId = 123;
      const entity = EntityUtils.createBaseEntity(customId);
      
      expect(entity.id).toBe(customId);
    });

    it('should update timestamp', () => {
      const entity = EntityUtils.createBaseEntity();
      const originalUpdatedAt = entity.updatedAt;
      
      // Wait a small amount to ensure different timestamp
      setTimeout(() => {
        const updatedEntity = EntityUtils.updateTimestamp(entity);
        expect(updatedEntity.updatedAt.getTime()).toBeGreaterThan(originalUpdatedAt.getTime());
      }, 1);
    });

    it('should validate entity correctly', () => {
      const validEntity = EntityUtils.createBaseEntity();
      expect(EntityUtils.isEntityValid(validEntity)).toBe(true);
      
      const invalidEntity = { id: 0, createdAt: null, updatedAt: null } as any;
      expect(EntityUtils.isEntityValid(invalidEntity)).toBe(false);
    });
  });

  describe('User Entity', () => {
    it('should create user with defaults', () => {
      const user = UserFactory.create({});
      
      expect(user.name).toBe('Unknown User');
      expect(user.email).toBe('user@example.com');
      expect(user.active).toBe(true);
      expect(user.role).toBe(UserRole.USER);
      expect(user.id).toBeDefined();
    });

    it('should create user with custom data', () => {
      const userData = {
        name: 'John Doe',
        email: 'john@example.com',
        active: false,
        role: UserRole.ADMIN
      };
      
      const user = UserFactory.create(userData);
      
      expect(user.name).toBe('John Doe');
      expect(user.email).toBe('john@example.com');
      expect(user.active).toBe(false);
      expect(user.role).toBe(UserRole.ADMIN);
    });

    it('should create user with profile', () => {
      const userData = { name: 'John Doe' };
      const profileData = {
        firstName: 'John',
        lastName: 'Doe',
        bio: 'Software Developer'
      };
      
      const user = UserFactory.createWithProfile(userData, profileData);
      
      expect(user.profile).toBeDefined();
      expect(user.profile!.firstName).toBe('John');
      expect(user.profile!.lastName).toBe('Doe');
      expect(user.profile!.bio).toBe('Software Developer');
    });

    it('should validate user roles', () => {
      expect(Object.values(UserRole)).toContain(UserRole.ADMIN);
      expect(Object.values(UserRole)).toContain(UserRole.USER);
      expect(Object.values(UserRole)).toContain(UserRole.MODERATOR);
      expect(Object.values(UserRole)).toContain(UserRole.GUEST);
    });
  });

  describe('Post Entity', () => {
    it('should create post with defaults', () => {
      const post = PostFactory.create({});
      
      expect(post.title).toBe('Untitled Post');
      expect(post.content).toBe('');
      expect(post.published).toBe(false);
      expect(post.authorId).toBe(1);
      expect(post.tags).toEqual([]);
      expect(post.views).toBe(0);
      expect(post.likes).toBe(0);
    });

    it('should create post with custom data', () => {
      const postData = {
        title: 'My First Post',
        content: 'This is the content',
        published: true,
        authorId: 42,
        tags: ['javascript', 'angular'],
        views: 100,
        likes: 25
      };
      
      const post = PostFactory.create(postData);
      
      expect(post.title).toBe('My First Post');
      expect(post.content).toBe('This is the content');
      expect(post.published).toBe(true);
      expect(post.authorId).toBe(42);
      expect(post.tags).toEqual(['javascript', 'angular']);
      expect(post.views).toBe(100);
      expect(post.likes).toBe(25);
    });

    it('should handle post with author relationship', () => {
      const author = UserFactory.create({ name: 'John Author' });
      const post = PostFactory.create({
        title: 'Test Post',
        authorId: author.id,
        author: author
      });
      
      expect(post.author).toBe(author);
      expect(post.authorId).toBe(author.id);
    });
  });

  describe('Todo Entity', () => {
    it('should create todo with defaults', () => {
      const todo = TodoFactory.create({});
      
      expect(todo.title).toBe('New Todo');
      expect(todo.completed).toBe(false);
      expect(todo.priority).toBe(TodoPriority.MEDIUM);
      expect(todo.userId).toBe(1);
      expect(todo.description).toBeUndefined();
      expect(todo.dueDate).toBeUndefined();
    });

    it('should create todo with custom data', () => {
      const dueDate = new Date('2025-12-31');
      const todoData = {
        title: 'Complete project',
        description: 'Finish the Angular project',
        completed: true,
        dueDate: dueDate,
        priority: TodoPriority.HIGH,
        userId: 42
      };
      
      const todo = TodoFactory.create(todoData);
      
      expect(todo.title).toBe('Complete project');
      expect(todo.description).toBe('Finish the Angular project');
      expect(todo.completed).toBe(true);
      expect(todo.dueDate).toBe(dueDate);
      expect(todo.priority).toBe(TodoPriority.HIGH);
      expect(todo.userId).toBe(42);
    });

    it('should validate todo priorities', () => {
      expect(Object.values(TodoPriority)).toContain(TodoPriority.LOW);
      expect(Object.values(TodoPriority)).toContain(TodoPriority.MEDIUM);
      expect(Object.values(TodoPriority)).toContain(TodoPriority.HIGH);
      expect(Object.values(TodoPriority)).toContain(TodoPriority.URGENT);
    });
  });

  describe('Entity Utils', () => {
    let entities: Post[];

    beforeEach(() => {
      entities = [
        PostFactory.create({ title: 'Post 1', createdAt: new Date('2025-01-01') }),
        PostFactory.create({ title: 'Post 2', createdAt: new Date('2025-01-15') }),
        PostFactory.create({ title: 'Post 3', createdAt: new Date('2025-02-01') })
      ];
    });

    it('should sort entities by created date ascending', () => {
      const sorted = EntityUtils.sortByCreatedDate(entities, true);
      
      expect(sorted[0].title).toBe('Post 1');
      expect(sorted[1].title).toBe('Post 2');
      expect(sorted[2].title).toBe('Post 3');
    });

    it('should sort entities by created date descending', () => {
      const sorted = EntityUtils.sortByCreatedDate(entities, false);
      
      expect(sorted[0].title).toBe('Post 3');
      expect(sorted[1].title).toBe('Post 2');
      expect(sorted[2].title).toBe('Post 1');
    });

    it('should filter entities by date range', () => {
      const startDate = new Date('2025-01-10');
      const endDate = new Date('2025-01-20');
      
      const filtered = EntityUtils.filterByDateRange(entities, startDate, endDate);
      
      expect(filtered).toHaveLength(1);
      expect(filtered[0].title).toBe('Post 2');
    });

    it('should group entities by property', () => {
      const todos = [
        TodoFactory.create({ priority: TodoPriority.HIGH, title: 'Todo 1' }),
        TodoFactory.create({ priority: TodoPriority.HIGH, title: 'Todo 2' }),
        TodoFactory.create({ priority: TodoPriority.LOW, title: 'Todo 3' })
      ];
      
      const grouped = EntityUtils.groupByProperty(todos, 'priority');
      
      expect(grouped[TodoPriority.HIGH]).toHaveLength(2);
      expect(grouped[TodoPriority.LOW]).toHaveLength(1);
      expect(grouped[TodoPriority.HIGH][0].title).toBe('Todo 1');
    });
  });

  describe('API Response Types', () => {
    it('should create valid API response', () => {
      const response: ApiResponse<User> = {
        data: UserFactory.create({ name: 'Test User' }),
        message: 'User retrieved successfully',
        success: true,
        timestamp: new Date()
      };
      
      expect(response.success).toBe(true);
      expect(response.data.name).toBe('Test User');
      expect(response.message).toBeDefined();
      expect(response.timestamp).toBeInstanceOf(Date);
    });

    it('should create valid paginated response', () => {
      const users = [
        UserFactory.create({ name: 'User 1' }),
        UserFactory.create({ name: 'User 2' })
      ];
      
      const response: PaginatedResponse<User> = {
        data: users,
        message: 'Users retrieved successfully',
        success: true,
        timestamp: new Date(),
        pagination: {
          page: 1,
          limit: 10,
          total: 25,
          totalPages: 3,
          hasNext: true,
          hasPrev: false
        }
      };
      
      expect(response.data).toHaveLength(2);
      expect(response.pagination.page).toBe(1);
      expect(response.pagination.hasNext).toBe(true);
      expect(response.pagination.hasPrev).toBe(false);
    });

    it('should create valid error response', () => {
      const errorResponse: ErrorResponse = {
        error: {
          code: 'USER_NOT_FOUND',
          message: 'User with id 123 not found',
          details: { userId: 123 }
        },
        success: false,
        timestamp: new Date()
      };
      
      expect(errorResponse.success).toBe(false);
      expect(errorResponse.error.code).toBe('USER_NOT_FOUND');
      expect(errorResponse.error.details.userId).toBe(123);
    });
  });
});
