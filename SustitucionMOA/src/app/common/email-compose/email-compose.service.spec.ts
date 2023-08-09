import { TestBed } from '@angular/core/testing';

import { EmailComposeService } from './email-compose.service';

describe('EmailComposeService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: EmailComposeService = TestBed.get(EmailComposeService);
    expect(service).toBeTruthy();
  });
});
