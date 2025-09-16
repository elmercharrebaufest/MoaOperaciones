import { describe, it, expect, beforeEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { PLATFORM_ID } from '@angular/core';
import { I18nService, SupportedLocale } from './i18n.service';

describe('I18nService', () => {
  let service: I18nService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        I18nService,
        { provide: PLATFORM_ID, useValue: 'browser' }
      ]
    });
    service = TestBed.inject(I18nService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should have default locale', () => {
    expect(service.currentLocale()).toBeDefined();
    expect(['en', 'es']).toContain(service.currentLocale());
  });

  it('should have availableLocales', () => {
    expect(service.availableLocales).toBeDefined();
    expect(Array.isArray(service.availableLocales)).toBe(true);
    expect(service.availableLocales.length).toBeGreaterThan(0);
    expect(service.availableLocales).toContain('en');
    expect(service.availableLocales).toContain('es');
  });

  it('should change locale', () => {
    const newLocale: SupportedLocale = 'es';
    service.setLocale(newLocale);
    expect(service.currentLocale()).toBe(newLocale);
  });

  it('should translate keys', () => {
    const translation = service.translate('common.loading');
    expect(typeof translation).toBe('string');
    expect(translation.length).toBeGreaterThan(0);
  });

  it('should return key if translation not found', () => {
    const nonExistentKey = 'non.existent.key';
    const translation = service.translate(nonExistentKey);
    expect(translation).toBe(nonExistentKey);
  });

  it('should translate different languages', () => {
    // Test English
    service.setLocale('en');
    const englishTranslation = service.translate('common.loading');
    
    // Test Spanish
    service.setLocale('es');
    const spanishTranslation = service.translate('common.loading');
    
    expect(englishTranslation).not.toBe(spanishTranslation);
    expect(typeof englishTranslation).toBe('string');
    expect(typeof spanishTranslation).toBe('string');
  });
});
