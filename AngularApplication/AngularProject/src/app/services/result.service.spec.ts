import { TestBed, inject } from "@angular/core/testing";

import { ResultService as ResultServiceService } from "./result.service";

describe('ResultServiceService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ResultServiceService]
    });
  });

  it('should be created', inject([ResultServiceService], (service: ResultServiceService) => {
    expect(service).toBeTruthy();
  }));
});
