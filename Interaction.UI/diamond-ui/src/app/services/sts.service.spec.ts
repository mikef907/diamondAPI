import { TestBed } from '@angular/core/testing';

import { StsService } from './sts.service';

describe('StsService', () => {
  let service: StsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
