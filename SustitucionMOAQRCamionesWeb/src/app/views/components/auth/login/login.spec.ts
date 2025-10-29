import { describe, it, expect, beforeEach } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { provideRouter } from '@angular/router';

// Test constants
const TEST_DATA = {
  VALID_EMAIL: 'test@example.com',
  INVALID_EMAIL: 'invalid-email',
  EMPTY_VALUE: '',
  VALID_PASSWORD: 'password123',
  SHORT_PASSWORD: '12345',
  MIN_PASSWORD_LENGTH: 6
} as const;

const TEST_SELECTORS = {
  EMAIL_INPUT: '[data-testid="email-input"]',
  PASSWORD_INPUT: '[data-testid="password-input"]',
  SUBMIT_BTN: '[data-testid="submit-btn"]',
  ERROR_MESSAGE: '.error'
} as const;

const LOADING_STATES = {
  LOADING_TEXT: 'Logging in...',
  DEFAULT_TEXT: 'Login',
  SIMULATION_DELAY: 1000
} as const;

// Create a simplified test component for Login functionality
@Component({
  selector: 'app-login-test',
  template: `
    <div class="login-container">
      <h1>Login</h1>
      <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
        <div class="form-field">
          <input 
            type="email" 
            formControlName="email" 
            placeholder="Email"
            data-testid="email-input">
          @if (loginForm.get('email')?.invalid && loginForm.get('email')?.touched) {
            <div class="error">Email is required</div>
          }
        </div>
        
        <div class="form-field">
          <input 
            type="password" 
            formControlName="password" 
            placeholder="Password"
            data-testid="password-input">
          @if (loginForm.get('password')?.invalid && loginForm.get('password')?.touched) {
            <div class="error">Password is required</div>
          }
        </div>
        
        <button 
          type="submit" 
          [disabled]="loginForm.invalid || isLoading()"
          data-testid="submit-btn">
          {{isLoading() ? 'Logging in...' : 'Login'}}
        </button>
      </form>
    </div>
  `,
  standalone: true,
  imports: [ReactiveFormsModule]
})
class TestLogin {
  loginForm: FormGroup;
  public readonly isLoading = signal(false);
  private readonly fb = new FormBuilder();

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(TEST_DATA.MIN_PASSWORD_LENGTH)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      // Simulate login process
      setTimeout(() => {
        this.isLoading.set(false);
      }, LOADING_STATES.SIMULATION_DELAY);
    }
  }

  setFormValues(email: string, password: string): void {
    this.loginForm.patchValue({ email, password });
  }

  // Helper methods for testing
  markEmailAsTouched(): void {
    this.loginForm.get('email')?.markAsTouched();
  }

  markPasswordAsTouched(): void {
    this.loginForm.get('password')?.markAsTouched();
  }

  markAllFieldsAsTouched(): void {
    this.loginForm.markAllAsTouched();
  }
}

describe('Login', () => {
  let component: TestLogin;
  let fixture: ComponentFixture<TestLogin>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestLogin, ReactiveFormsModule],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(TestLogin);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('Component Creation', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
      expect(component).toBeInstanceOf(TestLogin);
    });

    it('should have login form', () => {
      expect(component.loginForm).toBeDefined();
      expect(component.loginForm).toBeInstanceOf(FormGroup);
      expect(component.loginForm.get('email')).toBeDefined();
      expect(component.loginForm.get('password')).toBeDefined();
    });

    it('should initialize with empty form', () => {
      expect(component.loginForm.get('email')?.value).toBe(TEST_DATA.EMPTY_VALUE);
      expect(component.loginForm.get('password')?.value).toBe(TEST_DATA.EMPTY_VALUE);
    });

    it('should initialize loading state as false', () => {
      expect(component.isLoading()).toBe(false);
    });
  });

  describe('Email Validation', () => {
    it('should validate email field', () => {
      const emailControl = component.loginForm.get('email');
      
      // Empty email should be invalid
      emailControl?.setValue(TEST_DATA.EMPTY_VALUE);
      component.markEmailAsTouched();
      expect(emailControl?.invalid).toBe(true);
      expect(emailControl?.hasError('required')).toBe(true);
      
      // Invalid email format should be invalid
      emailControl?.setValue(TEST_DATA.INVALID_EMAIL);
      expect(emailControl?.invalid).toBe(true);
      expect(emailControl?.hasError('email')).toBe(true);
      
      // Valid email should be valid
      emailControl?.setValue(TEST_DATA.VALID_EMAIL);
      expect(emailControl?.valid).toBe(true);
      expect(emailControl?.hasError('required')).toBe(false);
      expect(emailControl?.hasError('email')).toBe(false);
    });

    it('should show email error when field is touched and invalid', () => {
      const emailControl = component.loginForm.get('email');
      emailControl?.setValue(TEST_DATA.EMPTY_VALUE);
      component.markEmailAsTouched();
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const errorElement = compiled.querySelector(`${TEST_SELECTORS.EMAIL_INPUT} ~ ${TEST_SELECTORS.ERROR_MESSAGE}`);
      expect(errorElement).toBeTruthy();
    });
  });

  describe('Password Validation', () => {
    it('should validate password field', () => {
      const passwordControl = component.loginForm.get('password');
      
      // Empty password should be invalid
      passwordControl?.setValue(TEST_DATA.EMPTY_VALUE);
      component.markPasswordAsTouched();
      expect(passwordControl?.invalid).toBe(true);
      expect(passwordControl?.hasError('required')).toBe(true);
      
      // Short password should be invalid
      passwordControl?.setValue(TEST_DATA.SHORT_PASSWORD);
      expect(passwordControl?.invalid).toBe(true);
      expect(passwordControl?.hasError('minlength')).toBe(true);
      
      // Valid password should be valid
      passwordControl?.setValue(TEST_DATA.VALID_PASSWORD);
      expect(passwordControl?.valid).toBe(true);
      expect(passwordControl?.hasError('required')).toBe(false);
      expect(passwordControl?.hasError('minlength')).toBe(false);
    });

    it('should show password error when field is touched and invalid', () => {
      const passwordControl = component.loginForm.get('password');
      passwordControl?.setValue(TEST_DATA.EMPTY_VALUE);
      component.markPasswordAsTouched();
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const errorElement = compiled.querySelector(`${TEST_SELECTORS.PASSWORD_INPUT} ~ ${TEST_SELECTORS.ERROR_MESSAGE}`);
      expect(errorElement).toBeTruthy();
    });

    it('should enforce minimum password length', () => {
      const passwordControl = component.loginForm.get('password');
      passwordControl?.setValue(TEST_DATA.SHORT_PASSWORD);
      
      expect(passwordControl?.hasError('minlength')).toBe(true);
      const minLengthError = passwordControl?.getError('minlength');
      expect(minLengthError?.requiredLength).toBe(TEST_DATA.MIN_PASSWORD_LENGTH);
      expect(minLengthError?.actualLength).toBe(TEST_DATA.SHORT_PASSWORD.length);
    });
  });

  describe('Form Validation', () => {
    it('should validate form as a whole', () => {
      // Form should be invalid initially
      expect(component.loginForm.invalid).toBe(true);
      
      // Set valid values
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      expect(component.loginForm.valid).toBe(true);
    });

    it('should be invalid with only email filled', () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.EMPTY_VALUE);
      expect(component.loginForm.invalid).toBe(true);
    });

    it('should be invalid with only password filled', () => {
      component.setFormValues(TEST_DATA.EMPTY_VALUE, TEST_DATA.VALID_PASSWORD);
      expect(component.loginForm.invalid).toBe(true);
    });

    it('should be invalid with invalid email and valid password', () => {
      component.setFormValues(TEST_DATA.INVALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      expect(component.loginForm.invalid).toBe(true);
    });

    it('should be invalid with valid email and short password', () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.SHORT_PASSWORD);
      expect(component.loginForm.invalid).toBe(true);
    });
  });

  describe('Form Submission', () => {
    it('should handle submit with valid form', () => {
      // Set valid form values
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      
      expect(component.isLoading()).toBe(false);
      component.onSubmit();
      expect(component.isLoading()).toBe(true);
    });

    it('should not submit invalid form', () => {
      // Keep form invalid
      component.setFormValues(TEST_DATA.EMPTY_VALUE, TEST_DATA.EMPTY_VALUE);
      
      component.onSubmit();
      expect(component.isLoading()).toBe(false);
    });

    it('should not submit with invalid email', () => {
      component.setFormValues(TEST_DATA.INVALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      
      component.onSubmit();
      expect(component.isLoading()).toBe(false);
    });

    it('should not submit with short password', () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.SHORT_PASSWORD);
      
      component.onSubmit();
      expect(component.isLoading()).toBe(false);
    });

    it('should reset loading state after submission simulation', async () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      
      component.onSubmit();
      expect(component.isLoading()).toBe(true);
      
      // Wait for simulated async operation to complete
      await new Promise(resolve => setTimeout(resolve, LOADING_STATES.SIMULATION_DELAY + 100));
      expect(component.isLoading()).toBe(false);
    });
  });

  describe('DOM Rendering', () => {
    it('should render form elements', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      
      expect(compiled.querySelector(TEST_SELECTORS.EMAIL_INPUT)).toBeTruthy();
      expect(compiled.querySelector(TEST_SELECTORS.PASSWORD_INPUT)).toBeTruthy();
      expect(compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN)).toBeTruthy();
    });

    it('should render correct form input types', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const emailInput = compiled.querySelector(TEST_SELECTORS.EMAIL_INPUT) as HTMLInputElement;
      const passwordInput = compiled.querySelector(TEST_SELECTORS.PASSWORD_INPUT) as HTMLInputElement;
      
      expect(emailInput?.type).toBe('email');
      expect(passwordInput?.type).toBe('password');
    });

    it('should disable submit button when form is invalid', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const submitBtn = compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN) as HTMLButtonElement;
      
      expect(submitBtn?.disabled).toBe(true);
    });

    it('should enable submit button when form is valid', () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      const submitBtn = compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN) as HTMLButtonElement;
      
      expect(submitBtn?.disabled).toBe(false);
    });

    it('should disable submit button when loading', () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      component.onSubmit();
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      const submitBtn = compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN) as HTMLButtonElement;
      
      expect(submitBtn?.disabled).toBe(true);
    });

    it('should show correct button text when loading', () => {
      component.setFormValues(TEST_DATA.VALID_EMAIL, TEST_DATA.VALID_PASSWORD);
      component.onSubmit();
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement as HTMLElement;
      const submitBtn = compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN) as HTMLButtonElement;
      
      expect(submitBtn?.textContent?.trim()).toContain(LOADING_STATES.LOADING_TEXT);
    });

    it('should show correct button text when not loading', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const submitBtn = compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN) as HTMLButtonElement;
      
      expect(submitBtn?.textContent?.trim()).toContain(LOADING_STATES.DEFAULT_TEXT);
    });
  });

  describe('Accessibility', () => {
    it('should have proper form structure', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const form = compiled.querySelector('form');
      const emailInput = compiled.querySelector(TEST_SELECTORS.EMAIL_INPUT) as HTMLInputElement;
      const passwordInput = compiled.querySelector(TEST_SELECTORS.PASSWORD_INPUT) as HTMLInputElement;
      
      expect(form).toBeTruthy();
      expect(emailInput?.placeholder).toBe('Email');
      expect(passwordInput?.placeholder).toBe('Password');
    });

    it('should have proper button type', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const submitBtn = compiled.querySelector(TEST_SELECTORS.SUBMIT_BTN) as HTMLButtonElement;
      
      expect(submitBtn?.type).toBe('submit');
    });
  });
});
