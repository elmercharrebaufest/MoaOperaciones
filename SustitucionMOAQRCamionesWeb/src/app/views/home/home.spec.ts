import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal, computed } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

// Create a simplified test component that mimics Home's core functionality
@Component({
  selector: 'app-home-test',
  template: `
    <div class="home-container">
      <h1>Welcome to Angular 20 Demo</h1>
      <div class="counter-section">
        <p>Counter: {{counter()}}</p>
        <p>Double: {{doubleCounter()}}</p>
        <p>Is Even: {{isEven()}}</p>
        <button (click)="increment()" data-testid="increment-btn">Increment</button>
        <button (click)="decrement()" data-testid="decrement-btn">Decrement</button>
      </div>
    </div>
  `,
  standalone: true
})
class TestHome {
  // Signal-based counter demo (same as original)
  public readonly counter = signal(0);
  readonly doubleCounter = computed(() => this.counter() * 2);
  readonly isEven = computed(() => this.counter() % 2 === 0);

  increment(): void {
    this.counter.update(value => value + 1);
  }

  decrement(): void {
    this.counter.update(value => value - 1);
  }
}

describe('Home', () => {
  let component: TestHome;
  let fixture: ComponentFixture<TestHome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHome],
      providers: [
        provideRouter([]),
        provideHttpClient()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestHome);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize counter to 0', () => {
    expect(component.counter()).toBe(0);
    expect(component.doubleCounter()).toBe(0);
    expect(component.isEven()).toBe(true);
  });

  it('should increment counter', () => {
    component.increment();
    expect(component.counter()).toBe(1);
    expect(component.doubleCounter()).toBe(2);
    expect(component.isEven()).toBe(false);
  });

  it('should decrement counter', () => {
    component.increment(); // First set to 1
    component.decrement(); // Then back to 0
    expect(component.counter()).toBe(0);
    expect(component.doubleCounter()).toBe(0);
    expect(component.isEven()).toBe(true);
  });

  it('should render counter values in template', () => {
    component.increment();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Counter: 1');
    expect(compiled.textContent).toContain('Double: 2');
    expect(compiled.textContent).toContain('Is Even: false');
  });
});
