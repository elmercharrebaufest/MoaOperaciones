import { TestBed } from '@angular/core/testing';

import { DomainService } from './domain';

describe('DomainService', () => {
  let service: DomainService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DomainService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should make greeting with default name', () => {
    const greeting = service.makeGreeting();
    expect(greeting.message).toBe('Hola, Angularista!');
    expect(greeting.author).toBe('CleanArchitecture');
  });

  it('should make greeting with custom name', () => {
    const greeting = service.makeGreeting('Juan');
    expect(greeting.message).toBe('Hola, Juan!');
    expect(greeting.author).toBe('CleanArchitecture');
  });
});
