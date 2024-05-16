import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyControlsComponent } from './my-controls.component';

describe('MyControlsComponent', () => {
  let component: MyControlsComponent;
  let fixture: ComponentFixture<MyControlsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyControlsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyControlsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
