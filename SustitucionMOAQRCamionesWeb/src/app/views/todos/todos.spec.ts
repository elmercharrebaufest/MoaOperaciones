import { describe, it, expect, beforeEach } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal, computed } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

// Test constants
const TEST_DATA = {
  LOADING_DELAY: 100,
  TEST_TIMEOUT: 150,
  COMPLETION_PERCENTAGES: {
    ZERO: 0,
    THIRTY_THREE: 33,
    SIXTY_SEVEN: 67,
    ONE_HUNDRED: 100
  },
  TODO_COUNTS: {
    ZERO: 0,
    ONE: 1,
    TWO: 2,
    THREE: 3,
    FOUR: 4
  },
  TODO_IDS: {
    FIRST: 1,
    SECOND: 2,
    THIRD: 3,
    FOURTH: 4
  },
  USER_IDS: {
    FIRST_USER: 1,
    SECOND_USER: 2
  }
} as const;

// Entity interface definition for task items
interface Todo {
  id: number;
  title: string;
  completed: boolean;
  userId: number;
}

// Create a simplified test component for Task functionality
@Component({
  selector: 'app-todos-test',
  template: `
    <div class="todos-container">
      <h1>Task List</h1>
      <div class="todos-stats">
        <p>Total: {{todos().length}}</p>
        <p>Completed: {{completedTodos().length}}</p>
        <p>Completion: {{completionPercentage()}}%</p>
      </div>
      <div class="todos-list">
        @for (task of todos(); track task.id) {
          <div class="todo-item" [class.completed]="task.completed">
            {{task.title}}
          </div>
        }
      </div>
      @if (isLoading()) {
        <div class="loading">Loading tasks...</div>
      }
    </div>
  `,
  standalone: true,
  imports: []
})
class TestTodos {
  public readonly isLoading = signal(false);
  public readonly todos = signal<Todo[]>([]);

  // Computed signal for completed todos
  readonly completedTodos = computed(() => 
    this.todos().filter(todo => todo.completed)
  );

  // Computed signal for completion percentage
  readonly completionPercentage = computed(() => {
    const total = this.todos().length;
    const completed = this.completedTodos().length;
    return total > 0 ? Math.round((completed / total) * 100) : 0;
  });

  loadTodos(mockTodos: Todo[] = []): void {
    this.isLoading.set(true);
    setTimeout(() => {
      this.todos.set(mockTodos);
      this.isLoading.set(false);
    }, TEST_DATA.LOADING_DELAY);
  }

  toggleTodo(id: number): void {
    const currentTodos = this.todos();
    const updatedTodos = currentTodos.map(task => 
      task.id === id ? { ...task, completed: !task.completed } : task
    );
    this.todos.set(updatedTodos);
  }

  // Helper methods for testing
  getTotalCount(): number {
    return this.todos().length;
  }

  getCompletedCount(): number {
    return this.completedTodos().length;
  }

  getPendingCount(): number {
    return this.todos().length - this.completedTodos().length;
  }

  clearAllTodos(): void {
    this.todos.set([]);
  }

  addTodo(task: Todo): void {
    const currentTodos = this.todos();
    this.todos.set([...currentTodos, task]);
  }

  markAllCompleted(): void {
    const currentTodos = this.todos();
    const updatedTodos = currentTodos.map(task => ({ ...task, completed: true }));
    this.todos.set(updatedTodos);
  }
}

describe('Todos', () => {
  let component: TestTodos;
  let fixture: ComponentFixture<TestTodos>;

  const mockTodos: Todo[] = [
    { id: TEST_DATA.TODO_IDS.FIRST, title: 'Test task 1', completed: false, userId: TEST_DATA.USER_IDS.FIRST_USER },
    { id: TEST_DATA.TODO_IDS.SECOND, title: 'Test task 2', completed: true, userId: TEST_DATA.USER_IDS.FIRST_USER },
    { id: TEST_DATA.TODO_IDS.THIRD, title: 'Test task 3', completed: false, userId: TEST_DATA.USER_IDS.FIRST_USER }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestTodos],
      providers: [provideHttpClient()]
    }).compileComponents();

    fixture = TestBed.createComponent(TestTodos);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('Component Creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize with empty todos', () => {
      expect(component.todos()).toEqual([]);
      expect(component.completedTodos()).toEqual([]);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ZERO);
    });

    it('should initialize with default state', () => {
      expect(component.isLoading()).toBe(false);
      expect(component.getTotalCount()).toBe(TEST_DATA.TODO_COUNTS.ZERO);
      expect(component.getCompletedCount()).toBe(TEST_DATA.TODO_COUNTS.ZERO);
      expect(component.getPendingCount()).toBe(TEST_DATA.TODO_COUNTS.ZERO);
    });
  });

  describe('Completion Percentage Calculations', () => {
    it('should calculate completion percentage correctly', () => {
      component.todos.set(mockTodos);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.THIRTY_THREE);
    });

    it('should return zero percentage for empty task list', () => {
      component.todos.set([]);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ZERO);
    });

    it('should return 100% when all tasks are completed', () => {
      const allCompletedTasks: Todo[] = [
        { id: TEST_DATA.TODO_IDS.FIRST, title: 'Completed task 1', completed: true, userId: TEST_DATA.USER_IDS.FIRST_USER },
        { id: TEST_DATA.TODO_IDS.SECOND, title: 'Completed task 2', completed: true, userId: TEST_DATA.USER_IDS.FIRST_USER }
      ];
      component.todos.set(allCompletedTasks);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ONE_HUNDRED);
    });

    it('should handle single task scenarios', () => {
      const singleIncompleteTask: Todo[] = [
        { id: TEST_DATA.TODO_IDS.FIRST, title: 'Single task', completed: false, userId: TEST_DATA.USER_IDS.FIRST_USER }
      ];
      component.todos.set(singleIncompleteTask);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ZERO);

      const singleCompleteTask: Todo[] = [
        { id: TEST_DATA.TODO_IDS.FIRST, title: 'Single task', completed: true, userId: TEST_DATA.USER_IDS.FIRST_USER }
      ];
      component.todos.set(singleCompleteTask);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ONE_HUNDRED);
    });
  });

  describe('Task Filtering', () => {
    beforeEach(() => {
      component.todos.set(mockTodos);
    });

    it('should filter completed todos correctly', () => {
      const completed = component.completedTodos();
      expect(completed).toHaveLength(TEST_DATA.TODO_COUNTS.ONE);
      expect(completed[0].id).toBe(TEST_DATA.TODO_IDS.SECOND);
    });

    it('should count completed tasks accurately', () => {
      expect(component.getCompletedCount()).toBe(TEST_DATA.TODO_COUNTS.ONE);
      expect(component.getPendingCount()).toBe(TEST_DATA.TODO_COUNTS.TWO);
      expect(component.getTotalCount()).toBe(TEST_DATA.TODO_COUNTS.THREE);
    });

    it('should update counts when tasks are toggled', () => {
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      expect(component.getCompletedCount()).toBe(TEST_DATA.TODO_COUNTS.TWO);
      expect(component.getPendingCount()).toBe(TEST_DATA.TODO_COUNTS.ONE);
    });
  });

  describe('Task Management', () => {
    it('should toggle todo completion status', () => {
      component.todos.set(mockTodos);
      
      // Toggle first task to completed
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      expect(component.todos()[0].completed).toBe(true);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.SIXTY_SEVEN);
    });

    it('should toggle completed task back to incomplete', () => {
      component.todos.set(mockTodos);
      
      // Toggle already completed task back to incomplete
      component.toggleTodo(TEST_DATA.TODO_IDS.SECOND);
      expect(component.todos()[1].completed).toBe(false);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ZERO);
    });

    it('should add new task correctly', () => {
      const newTask: Todo = {
        id: TEST_DATA.TODO_IDS.FOURTH,
        title: 'New task',
        completed: false,
        userId: TEST_DATA.USER_IDS.FIRST_USER
      };
      
      component.todos.set(mockTodos);
      component.addTodo(newTask);
      
      expect(component.getTotalCount()).toBe(TEST_DATA.TODO_COUNTS.FOUR);
      expect(component.todos()).toContain(newTask);
    });

    it('should clear all tasks', () => {
      component.todos.set(mockTodos);
      component.clearAllTodos();
      
      expect(component.getTotalCount()).toBe(TEST_DATA.TODO_COUNTS.ZERO);
      expect(component.todos()).toEqual([]);
    });

    it('should mark all tasks as completed', () => {
      component.todos.set(mockTodos);
      component.markAllCompleted();
      
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.ONE_HUNDRED);
      expect(component.getCompletedCount()).toBe(TEST_DATA.TODO_COUNTS.THREE);
      expect(component.getPendingCount()).toBe(TEST_DATA.TODO_COUNTS.ZERO);
    });
  });

  describe('Loading State Management', () => {
    it('should handle loading state correctly', () => {
      expect(component.isLoading()).toBe(false);
      
      component.isLoading.set(true);
      expect(component.isLoading()).toBe(true);
    });

    it('should manage loading during task loading', async () => {
      expect(component.isLoading()).toBe(false);
      
      component.loadTodos(mockTodos);
      expect(component.isLoading()).toBe(true);
      
      await new Promise(resolve => setTimeout(resolve, TEST_DATA.TEST_TIMEOUT));
      
      expect(component.isLoading()).toBe(false);
      expect(component.getTotalCount()).toBe(TEST_DATA.TODO_COUNTS.THREE);
    });

    it('should load empty task array', async () => {
      component.loadTodos([]);
      
      await new Promise(resolve => setTimeout(resolve, TEST_DATA.TEST_TIMEOUT));
      
      expect(component.isLoading()).toBe(false);
      expect(component.getTotalCount()).toBe(TEST_DATA.TODO_COUNTS.ZERO);
    });
  });

  describe('Template Rendering', () => {
    it('should render todos count in template', () => {
      component.todos.set(mockTodos);
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.textContent).toContain('Total: 3');
      expect(compiled.textContent).toContain('Completed: 1');
    });

    it('should update template when completion changes', () => {
      component.todos.set(mockTodos);
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.textContent).toContain('Total: 3');
      expect(compiled.textContent).toContain('Completed: 2');
      expect(compiled.textContent).toContain('Completion: 67%');
    });

    it('should handle empty state in template', () => {
      component.todos.set([]);
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.textContent).toContain('Total: 0');
      expect(compiled.textContent).toContain('Completed: 0');
      expect(compiled.textContent).toContain('Completion: 0%');
    });

    it('should display loading state', () => {
      component.isLoading.set(true);
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      expect(compiled.textContent).toContain('Loading tasks...');
    });
  });

  describe('Error Handling and Edge Cases', () => {
    it('should handle invalid task ID during toggle', () => {
      component.todos.set(mockTodos);
      const originalTasks = [...component.todos()];
      
      // Try to toggle non-existent task
      component.toggleTodo(999);
      
      expect(component.todos()).toEqual(originalTasks);
    });

    it('should handle tasks with same ID correctly', () => {
      const duplicateIdTasks: Todo[] = [
        { id: TEST_DATA.TODO_IDS.FIRST, title: 'Task 1', completed: false, userId: TEST_DATA.USER_IDS.FIRST_USER },
        { id: TEST_DATA.TODO_IDS.FIRST, title: 'Task 2', completed: true, userId: TEST_DATA.USER_IDS.FIRST_USER }
      ];
      
      component.todos.set(duplicateIdTasks);
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      
      // Both tasks with same ID should be toggled
      expect(component.todos()[0].completed).toBe(true);
      expect(component.todos()[1].completed).toBe(false);
    });

    it('should maintain data integrity during operations', () => {
      const originalTasks = [...mockTodos];
      component.todos.set(mockTodos);
      
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      
      // After toggling twice, should return to original state
      expect(component.todos()[0].completed).toBe(originalTasks[0].completed);
      expect(component.todos()[0].title).toBe(originalTasks[0].title);
      expect(component.todos()[0].userId).toBe(originalTasks[0].userId);
    });

    it('should handle rapid state changes', () => {
      component.todos.set(mockTodos);
      
      // Rapid toggles
      component.toggleTodo(TEST_DATA.TODO_IDS.FIRST);
      component.toggleTodo(TEST_DATA.TODO_IDS.SECOND);
      component.toggleTodo(TEST_DATA.TODO_IDS.THIRD);
      
      expect(component.getCompletedCount()).toBe(TEST_DATA.TODO_COUNTS.TWO);
      expect(component.completionPercentage()).toBe(TEST_DATA.COMPLETION_PERCENTAGES.SIXTY_SEVEN);
    });
  });
});
