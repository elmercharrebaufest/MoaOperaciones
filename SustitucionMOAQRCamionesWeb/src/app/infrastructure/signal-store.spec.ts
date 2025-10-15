import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { signal, computed, Signal } from '@angular/core';

// Mock interfaces for signal store
interface User {
  id: number;
  name: string;
  email: string;
  active: boolean;
}

interface Todo {
  id: number;
  title: string;
  completed: boolean;
  userId: number;
}

interface AppState {
  users: User[];
  todos: Todo[];
  loading: boolean;
  error: string | null;
}

// Signal Store implementation for testing
class TestSignalStore {
  // Private signals for state management
  private readonly _users = signal<User[]>([]);
  private readonly _todos = signal<Todo[]>([]);
  private readonly _loading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);

  // Public read-only signals
  readonly users: Signal<User[]> = this._users.asReadonly();
  readonly todos: Signal<Todo[]> = this._todos.asReadonly();
  readonly loading: Signal<boolean> = this._loading.asReadonly();
  readonly error: Signal<string | null> = this._error.asReadonly();

  // Computed signals
  readonly activeUsers = computed(() => 
    this._users().filter(user => user.active)
  );

  readonly completedTodos = computed(() =>
    this._todos().filter(todo => todo.completed)
  );

  readonly todosByUser = computed(() => {
    const users = this._users();
    const todos = this._todos();
    
    return users.map(user => ({
      user,
      todos: todos.filter(todo => todo.userId === user.id),
      completedCount: todos.filter(todo => todo.userId === user.id && todo.completed).length
    }));
  });

  readonly hasData = computed(() => 
    this._users().length > 0 || this._todos().length > 0
  );

  readonly state = computed<AppState>(() => ({
    users: this._users(),
    todos: this._todos(),
    loading: this._loading(),
    error: this._error()
  }));

  // Actions
  setUsers(users: User[]): void {
    this._users.set(users);
    this._error.set(null);
  }

  addUser(user: User): void {
    const currentUsers = this._users();
    this._users.set([...currentUsers, user]);
  }

  updateUser(userId: number, updates: Partial<User>): void {
    const currentUsers = this._users();
    const updatedUsers = currentUsers.map(user =>
      user.id === userId ? { ...user, ...updates } : user
    );
    this._users.set(updatedUsers);
  }

  removeUser(userId: number): void {
    const currentUsers = this._users();
    this._users.set(currentUsers.filter(user => user.id !== userId));
  }

  setTodos(todos: Todo[]): void {
    this._todos.set(todos);
    this._error.set(null);
  }

  addTodo(todo: Todo): void {
    const currentTodos = this._todos();
    this._todos.set([...currentTodos, todo]);
  }

  updateTodo(todoId: number, updates: Partial<Todo>): void {
    const currentTodos = this._todos();
    const updatedTodos = currentTodos.map(todo =>
      todo.id === todoId ? { ...todo, ...updates } : todo
    );
    this._todos.set(updatedTodos);
  }

  toggleTodo(todoId: number): void {
    const currentTodos = this._todos();
    const updatedTodos = currentTodos.map(todo =>
      todo.id === todoId ? { ...todo, completed: !todo.completed } : todo
    );
    this._todos.set(updatedTodos);
  }

  removeTodo(todoId: number): void {
    const currentTodos = this._todos();
    this._todos.set(currentTodos.filter(todo => todo.id !== todoId));
  }

  setLoading(loading: boolean): void {
    this._loading.set(loading);
  }

  setError(error: string | null): void {
    this._error.set(error);
  }

  reset(): void {
    this._users.set([]);
    this._todos.set([]);
    this._loading.set(false);
    this._error.set(null);
  }

  // Bulk operations
  bulkUpdateUsers(updates: { id: number; updates: Partial<User> }[]): void {
    const currentUsers = this._users();
    const updatedUsers = currentUsers.map(user => {
      const update = updates.find(u => u.id === user.id);
      return update ? { ...user, ...update.updates } : user;
    });
    this._users.set(updatedUsers);
  }

  bulkToggleTodos(todoIds: number[]): void {
    const currentTodos = this._todos();
    const updatedTodos = currentTodos.map(todo =>
      todoIds.includes(todo.id) ? { ...todo, completed: !todo.completed } : todo
    );
    this._todos.set(updatedTodos);
  }
}

describe('SignalStore', () => {
  let store: TestSignalStore;

  const mockUsers: User[] = [
    { id: 1, name: 'John Doe', email: 'john@example.com', active: true },
    { id: 2, name: 'Jane Smith', email: 'jane@example.com', active: false },
    { id: 3, name: 'Bob Johnson', email: 'bob@example.com', active: true }
  ];

  const mockTodos: Todo[] = [
    { id: 1, title: 'Task 1', completed: false, userId: 1 },
    { id: 2, title: 'Task 2', completed: true, userId: 1 },
    { id: 3, title: 'Task 3', completed: false, userId: 2 },
    { id: 4, title: 'Task 4', completed: true, userId: 3 }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = new TestSignalStore();
  });

  describe('Initial State', () => {
    it('should initialize with empty state', () => {
      expect(store.users()).toEqual([]);
      expect(store.todos()).toEqual([]);
      expect(store.loading()).toBe(false);
      expect(store.error()).toBe(null);
    });

    it('should have computed properties working with empty state', () => {
      expect(store.activeUsers()).toEqual([]);
      expect(store.completedTodos()).toEqual([]);
      expect(store.todosByUser()).toEqual([]);
      expect(store.hasData()).toBe(false);
    });
  });

  describe('User Management', () => {
    it('should set users', () => {
      store.setUsers(mockUsers);
      expect(store.users()).toEqual(mockUsers);
      expect(store.error()).toBe(null);
    });

    it('should add a user', () => {
      store.setUsers(mockUsers);
      const newUser: User = { id: 4, name: 'Alice Brown', email: 'alice@example.com', active: true };
      
      store.addUser(newUser);
      
      expect(store.users()).toHaveLength(4);
      expect(store.users()).toContain(newUser);
    });

    it('should update a user', () => {
      store.setUsers(mockUsers);
      
      store.updateUser(1, { name: 'John Updated', active: false });
      
      const updatedUser = store.users().find(u => u.id === 1);
      expect(updatedUser?.name).toBe('John Updated');
      expect(updatedUser?.active).toBe(false);
      expect(updatedUser?.email).toBe('john@example.com'); // Should preserve other properties
    });

    it('should remove a user', () => {
      store.setUsers(mockUsers);
      
      store.removeUser(2);
      
      expect(store.users()).toHaveLength(2);
      expect(store.users().find(u => u.id === 2)).toBeUndefined();
    });

    it('should compute active users correctly', () => {
      store.setUsers(mockUsers);
      
      const activeUsers = store.activeUsers();
      expect(activeUsers).toHaveLength(2);
      expect(activeUsers.every(u => u.active)).toBe(true);
    });
  });

  describe('Todo Management', () => {
    it('should set todos', () => {
      store.setTodos(mockTodos);
      expect(store.todos()).toEqual(mockTodos);
      expect(store.error()).toBe(null);
    });

    it('should add a todo', () => {
      store.setTodos(mockTodos);
      const newTodo: Todo = { id: 5, title: 'New Task', completed: false, userId: 1 };
      
      store.addTodo(newTodo);
      
      expect(store.todos()).toHaveLength(5);
      expect(store.todos()).toContain(newTodo);
    });

    it('should update a todo', () => {
      store.setTodos(mockTodos);
      
      store.updateTodo(1, { title: 'Updated Task', completed: true });
      
      const updatedTodo = store.todos().find(t => t.id === 1);
      expect(updatedTodo?.title).toBe('Updated Task');
      expect(updatedTodo?.completed).toBe(true);
      expect(updatedTodo?.userId).toBe(1); // Should preserve other properties
    });

    it('should toggle a todo', () => {
      store.setTodos(mockTodos);
      const originalTodo = store.todos().find(t => t.id === 1);
      const originalCompleted = originalTodo?.completed;
      
      store.toggleTodo(1);
      
      const toggledTodo = store.todos().find(t => t.id === 1);
      expect(toggledTodo?.completed).toBe(!originalCompleted);
    });

    it('should remove a todo', () => {
      store.setTodos(mockTodos);
      
      store.removeTodo(2);
      
      expect(store.todos()).toHaveLength(3);
      expect(store.todos().find(t => t.id === 2)).toBeUndefined();
    });

    it('should compute completed todos correctly', () => {
      store.setTodos(mockTodos);
      
      const completedTodos = store.completedTodos();
      expect(completedTodos).toHaveLength(2);
      expect(completedTodos.every(t => t.completed)).toBe(true);
    });
  });

  describe('Computed Properties', () => {
    beforeEach(() => {
      store.setUsers(mockUsers);
      store.setTodos(mockTodos);
    });

    it('should compute todos by user correctly', () => {
      const todosByUser = store.todosByUser();
      
      expect(todosByUser).toHaveLength(3);
      
      const user1Data = todosByUser.find(item => item.user.id === 1);
      expect(user1Data?.todos).toHaveLength(2);
      expect(user1Data?.completedCount).toBe(1);
      
      const user2Data = todosByUser.find(item => item.user.id === 2);
      expect(user2Data?.todos).toHaveLength(1);
      expect(user2Data?.completedCount).toBe(0);
    });

    it('should compute hasData correctly', () => {
      expect(store.hasData()).toBe(true);
      
      store.reset();
      expect(store.hasData()).toBe(false);
    });

    it('should compute complete state correctly', () => {
      const state = store.state();
      
      expect(state.users).toEqual(mockUsers);
      expect(state.todos).toEqual(mockTodos);
      expect(state.loading).toBe(false);
      expect(state.error).toBe(null);
    });
  });

  describe('State Management', () => {
    it('should set loading state', () => {
      store.setLoading(true);
      expect(store.loading()).toBe(true);
      
      store.setLoading(false);
      expect(store.loading()).toBe(false);
    });

    it('should set error state', () => {
      const errorMessage = 'Something went wrong';
      store.setError(errorMessage);
      expect(store.error()).toBe(errorMessage);
      
      store.setError(null);
      expect(store.error()).toBe(null);
    });

    it('should reset all state', () => {
      store.setUsers(mockUsers);
      store.setTodos(mockTodos);
      store.setLoading(true);
      store.setError('Error');
      
      store.reset();
      
      expect(store.users()).toEqual([]);
      expect(store.todos()).toEqual([]);
      expect(store.loading()).toBe(false);
      expect(store.error()).toBe(null);
    });
  });

  describe('Bulk Operations', () => {
    beforeEach(() => {
      store.setUsers(mockUsers);
      store.setTodos(mockTodos);
    });

    it('should bulk update users', () => {
      const updates = [
        { id: 1, updates: { active: false } },
        { id: 3, updates: { name: 'Bob Updated' } }
      ];
      
      store.bulkUpdateUsers(updates);
      
      const user1 = store.users().find(u => u.id === 1);
      const user3 = store.users().find(u => u.id === 3);
      
      expect(user1?.active).toBe(false);
      expect(user3?.name).toBe('Bob Updated');
    });

    it('should bulk toggle todos', () => {
      const todoIds = [1, 3];
      const originalTodo1 = store.todos().find(t => t.id === 1);
      const originalTodo3 = store.todos().find(t => t.id === 3);
      
      store.bulkToggleTodos(todoIds);
      
      const updatedTodo1 = store.todos().find(t => t.id === 1);
      const updatedTodo3 = store.todos().find(t => t.id === 3);
      
      expect(updatedTodo1?.completed).toBe(!originalTodo1?.completed);
      expect(updatedTodo3?.completed).toBe(!originalTodo3?.completed);
    });
  });
});
