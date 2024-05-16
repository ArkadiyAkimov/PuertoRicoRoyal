import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScrollAsistComponent } from './scroll-asist.component';

describe('ScrollAsistComponent', () => {
  let component: ScrollAsistComponent;
  let fixture: ComponentFixture<ScrollAsistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScrollAsistComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScrollAsistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
