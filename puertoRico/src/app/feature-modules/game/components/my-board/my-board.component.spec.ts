import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyBoardComponent } from './my-board.component';

describe('MyBoardComponent', () => {
  let component: MyBoardComponent;
  let fixture: ComponentFixture<MyBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyBoardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
