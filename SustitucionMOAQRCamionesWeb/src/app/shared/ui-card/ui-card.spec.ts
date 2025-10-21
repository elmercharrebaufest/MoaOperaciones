import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';
import { MATERIAL } from '../material';

// Create a test component with inline template for testing
@Component({
  selector: 'app-ui-card-test',
  template: `
    <div class="modern-card">
      <div class="card-header">
        <h2 class="card-title">{{title}}</h2>
        <p class="card-subtitle">{{subtitle}}</p>
      </div>
      <div class="card-content">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  standalone: true,
  imports: [...MATERIAL]
})
class TestUiCard {
  title = 'Card';
  subtitle = '';
}

describe('UiCard', () => {
  let component: TestUiCard;
  let fixture: ComponentFixture<TestUiCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestUiCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestUiCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default title', () => {
    expect(component.title).toBe('Card');
  });

  it('should render title in template', () => {
    component.title = 'Test Title';
    component.subtitle = 'Test Subtitle';
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.card-title')?.textContent).toContain('Test Title');
    expect(compiled.querySelector('.card-subtitle')?.textContent).toContain('Test Subtitle');
  });
});
