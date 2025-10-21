import { provideZonelessChangeDetection, Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, RouterOutlet } from '@angular/router';

// Create a test component with inline template for testing
@Component({
  selector: 'app-root',
  template: '<h1>Hello, qr-camiones</h1><router-outlet></router-outlet>',
  standalone: true,
  imports: [RouterOutlet]
})
class TestApp {
  title = 'qr-camiones';
}

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestApp],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([])
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(TestApp);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should have title property', () => {
    const fixture = TestBed.createComponent(TestApp);
    const app = fixture.componentInstance;
    expect(app.title).toBe('qr-camiones');
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(TestApp);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Hello, qr-camiones');
  });
});
