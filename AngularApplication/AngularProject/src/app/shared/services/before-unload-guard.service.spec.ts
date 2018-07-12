import { TestBed, inject } from '@angular/core/testing';

import { BeforeUnloadGuardService } from './before-unload-guard.service';

describe('BeforeUnloadGuardService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BeforeUnloadGuardService]
    });
  });

  it('should be created', inject([BeforeUnloadGuardService], (service: BeforeUnloadGuardService) => {
    expect(service).toBeTruthy();
  }));
});
