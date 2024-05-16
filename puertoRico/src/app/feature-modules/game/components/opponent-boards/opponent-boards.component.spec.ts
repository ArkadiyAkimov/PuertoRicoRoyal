import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OpponentBoardsComponent } from './opponent-boards.component';

describe('OpponentBoardsComponent', () => {
  let component: OpponentBoardsComponent;
  let fixture: ComponentFixture<OpponentBoardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OpponentBoardsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OpponentBoardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
