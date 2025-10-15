import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';

// Simple test component for basic functionality
@Component({
  selector: 'test-component',
  template: '<h1>Test Component</h1>',
  standalone: true
})
class TestComponent {
  title = 'Test';
}

describe('Basic Vitest Setup', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestComponent]
    }).compileComponents();
  });

  it('should create a basic component', () => {
    const fixture = TestBed.createComponent(TestComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
    expect(component.title).toBe('Test');
  });

  it('should render the component', () => {
    const fixture = TestBed.createComponent(TestComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toBe('Test Component');
  });
});
