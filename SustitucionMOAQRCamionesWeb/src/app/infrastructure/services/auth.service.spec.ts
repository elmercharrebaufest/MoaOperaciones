import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { PLATFORM_ID } from '@angular/core';
import { AuthService, LoginRequest } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: PLATFORM_ID, useValue: 'browser' }
      ]
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with user not authenticated', () => {
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBe(null);
  });

  it('should have required computed properties', () => {
    expect(service.isAdmin).toBeDefined();
    expect(service.isAuthenticated).toBeDefined();
    expect(service.currentUser).toBeDefined();
  });

  it('should have login method', () => {
    const loginRequest: LoginRequest = {
      email: 'test@test.com',
      password: 'password123'
    };
    
    expect(service.login).toBeDefined();
    expect(() => service.login(loginRequest)).not.toThrow();
  });

  it('should have logout method', () => {
    expect(service.logout).toBeDefined();
    expect(() => service.logout()).not.toThrow();
  });

  it('should update authentication state after logout', () => {
    service.logout();
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBe(null);
  });
});
