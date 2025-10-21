import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';
import { ApiService } from './api.service';

export interface Post {
  id: number;
  title: string;
  body: string;
  userId: number;
  author?: User;
}

export interface User {
  id: number;
  name: string;
  email: string;
  phone: string;
  website: string;
  company: {
    name: string;
    catchPhrase: string;
    bs: string;
  };
  address: {
    street: string;
    suite: string;
    city: string;
    zipcode: string;
    geo: {
      lat: string;
      lng: string;
    };
  };
}

export interface Todo {
  id: number;
  title: string;
  completed: boolean;
  userId: number;
}

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private readonly API_URL = 'https://jsonplaceholder.typicode.com';

  // Signals para manejo de estado
  private readonly _posts = signal<Post[]>([]);
  private readonly _users = signal<User[]>([]);
  private readonly _todos = signal<Todo[]>([]);
  private readonly _selectedUserId = signal<number | null>(null);

  // Computed signals
  readonly posts = this._posts.asReadonly();
  readonly users = this._users.asReadonly();
  readonly todos = this._todos.asReadonly();
  readonly selectedUser = computed(() => {
    const userId = this._selectedUserId();
    return userId ? this._users().find((u) => u.id === userId) : null;
  });
  readonly filteredPosts = computed(() => {
    const userId = this._selectedUserId();
    return userId ? this._posts().filter((p) => p.userId === userId) : this._posts();
  });
  readonly completedTodos = computed(() => this._todos().filter((todo) => todo.completed));
  readonly pendingTodos = computed(() => this._todos().filter((todo) => !todo.completed));

  constructor(public http: HttpClient, public apiService: ApiService) {}

  // Posts API
  getPosts(): Observable<Post[]> {
    return this.http.get<Post[]>(`${this.API_URL}/posts`).pipe(
      map((posts) => {
        this._posts.set(posts);
        return posts;
      }),
      catchError((error) => {
        console.error('Error fetching posts:', error);
        return of([]);
      })
    );
  }

  getPost(id: number): Observable<Post | null> {
    return this.http.get<Post>(`${this.API_URL}/posts/${id}`).pipe(
      catchError((error) => {
        console.error('Error fetching post:', error);
        return of(null);
      })
    );
  }

  createPost(post: Omit<Post, 'id'>): Observable<Post> {
    return this.http.post<Post>(`${this.API_URL}/posts`, post).pipe(
      map((newPost) => {
        this._posts.update((posts) => [...posts, newPost]);
        return newPost;
      })
    );
  }

  updatePost(id: number, post: Partial<Post>): Observable<Post> {
    return this.http.put<Post>(`${this.API_URL}/posts/${id}`, post).pipe(
      map((updatedPost) => {
        this._posts.update((posts) => posts.map((p) => (p.id === id ? updatedPost : p)));
        return updatedPost;
      })
    );
  }

  deletePost(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/posts/${id}`).pipe(
      map(() => {
        this._posts.update((posts) => posts.filter((p) => p.id !== id));
      })
    );
  }

  // Users API
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.API_URL}/users`).pipe(
      map((users) => {
        this._users.set(users);
        return users;
      }),
      catchError((error) => {
        console.error('Error fetching users:', error);
        return of([]);
      })
    );
  }

  getUser(id: number): Observable<User | null> {
    return this.http.get<User>(`${this.API_URL}/users/${id}`).pipe(
      catchError((error) => {
        console.error('Error fetching user:', error);
        return of(null);
      })
    );
  }

  // Todos API
  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(`${this.API_URL}/todos`).pipe(
      map((todos) => {
        this._todos.set(todos);
        return todos;
      }),
      catchError((error) => {
        console.error('Error fetching todos:', error);
        return of([]);
      })
    );
  }

  toggleTodo(id: number): Observable<Todo> {
    const todo = this._todos().find((t) => t.id === id);
    if (!todo) throw new Error('Todo not found');

    const updatedTodo = { ...todo, completed: !todo.completed };

    return this.http.put<Todo>(`${this.API_URL}/todos/${id}`, updatedTodo).pipe(
      map((result) => {
        this._todos.update((todos) => todos.map((t) => (t.id === id ? result : t)));
        return result;
      })
    );
  }

  actualizarSolp(nroSolp: string): Observable<any> {
    return this.apiService.actualizarSolp(nroSolp);
  }

  // State management methods
  setSelectedUser(userId: number | null): void {
    this._selectedUserId.set(userId);
  }

  resetData(): void {
    this._posts.set([]);
    this._users.set([]);
    this._todos.set([]);
    this._selectedUserId.set(null);
  }
}
