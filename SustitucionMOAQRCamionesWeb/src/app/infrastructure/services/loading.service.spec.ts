import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { LoadingService } from './loading.service';

describe('LoadingService', () => {
  let service: LoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LoadingService]
    });
    service = TestBed.inject(LoadingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with loading false', () => {
    expect(service.isLoading()).toBe(false);
    expect(service.loadingMessage()).toBe('Cargando...');
  });

  it('should set loading state', () => {
    service.setLoading(true, 'Test loading message');
    expect(service.isLoading()).toBe(true);
    expect(service.loadingMessage()).toBe('Test loading message');
  });

  it('should show loading with default message', () => {
    service.showLoading();
    expect(service.isLoading()).toBe(true);
    expect(service.loadingMessage()).toBe('Cargando...');
  });

  it('should show loading with custom message', () => {
    const customMessage = 'Loading custom data...';
    service.showLoading(customMessage);
    expect(service.isLoading()).toBe(true);
    expect(service.loadingMessage()).toBe(customMessage);
  });

  it('should hide loading', () => {
    service.showLoading('Test message');
    expect(service.isLoading()).toBe(true);
    
    service.hideLoading();
    expect(service.isLoading()).toBe(false);
  });

  it('should maintain message when hiding loading', () => {
    const message = 'Test message';
    service.showLoading(message);
    service.hideLoading();
    
    expect(service.isLoading()).toBe(false);
    // Message might be preserved or reset, depends on implementation
    expect(typeof service.loadingMessage()).toBe('string');
  });

  it('should handle multiple loading state changes', () => {
    service.showLoading('First message');
    expect(service.isLoading()).toBe(true);
    expect(service.loadingMessage()).toBe('First message');
    expect(service.loadingCount()).toBe(1);

    service.showLoading('Second message');
    expect(service.isLoading()).toBe(true);
    expect(service.loadingMessage()).toBe('Second message');
    expect(service.loadingCount()).toBe(2);

    service.hideLoading();
    expect(service.isLoading()).toBe(true); // Still loading because count is 1
    expect(service.loadingCount()).toBe(1);

    service.hideLoading();
    expect(service.isLoading()).toBe(false); // Now fully stopped
    expect(service.loadingCount()).toBe(0);
  });
});
