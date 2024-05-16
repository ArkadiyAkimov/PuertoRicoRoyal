import { TestBed } from '@angular/core/testing';

import { UserMainService } from './user-main.service';

describe('UserMainService', () => {
  let service: UserMainService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserMainService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
