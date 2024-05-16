import { TestBed } from '@angular/core/testing';

import { GameStartHttpService } from './game-start-http.service';

describe('GameStartHttpService', () => {
  let service: GameStartHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GameStartHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
