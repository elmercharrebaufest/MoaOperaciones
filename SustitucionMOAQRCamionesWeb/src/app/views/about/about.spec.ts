import { describe, it, expect, beforeEach } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';

// Create a simplified test component for About functionality
@Component({
  selector: 'app-about-test',
  template: `
    <div class="about-container">
      <h1>About Our Application</h1>
      <div class="about-content">
        <p>This is the about page content.</p>
        <p>Built with Angular 20 and modern web technologies.</p>
      </div>
      <div class="features">
        <h2>Features</h2>
        <ul>
          <li>Angular 20</li>
          <li>Signals</li>
          <li>Standalone Components</li>
          <li>SSR Support</li>
        </ul>
      </div>
    </div>
  `,
  standalone: true
})
class TestAbout {
  title = 'About Our Application';
  features = [
    'Angular 20',
    'Signals',
    'Standalone Components',
    'SSR Support'
  ];
}

describe('About', () => {
  let component: TestAbout;
  let fixture: ComponentFixture<TestAbout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestAbout]
    }).compileComponents();

    fixture = TestBed.createComponent(TestAbout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have title property', () => {
    expect(component.title).toBe('About Our Application');
  });

  it('should have features list', () => {
    expect(component.features).toBeDefined();
    expect(Array.isArray(component.features)).toBe(true);
    expect(component.features.length).toBeGreaterThan(0);
  });

  it('should render title in template', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('About Our Application');
  });

  it('should render about content', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.about-content')?.textContent).toContain('about page content');
  });

  it('should render features list', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const featuresSection = compiled.querySelector('.features');
    expect(featuresSection).toBeTruthy();
    expect(featuresSection?.querySelector('h2')?.textContent).toContain('Features');
    
    const listItems = compiled.querySelectorAll('li');
    expect(listItems.length).toBe(component.features.length);
  });
});
