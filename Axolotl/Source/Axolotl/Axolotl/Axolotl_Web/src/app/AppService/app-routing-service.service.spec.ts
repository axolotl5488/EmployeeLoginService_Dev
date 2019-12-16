import { TestBed } from '@angular/core/testing';

import { AppRoutingServiceService } from './app-routing-service.service';

describe('AppRoutingServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AppRoutingServiceService = TestBed.get(AppRoutingServiceService);
    expect(service).toBeTruthy();
  });
});
