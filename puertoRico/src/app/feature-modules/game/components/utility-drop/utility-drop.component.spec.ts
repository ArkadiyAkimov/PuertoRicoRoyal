import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UtilityDropComponent } from './utility-drop.component';

describe('UtilityDropComponent', () => {
  let component: UtilityDropComponent;
  let fixture: ComponentFixture<UtilityDropComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UtilityDropComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UtilityDropComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
