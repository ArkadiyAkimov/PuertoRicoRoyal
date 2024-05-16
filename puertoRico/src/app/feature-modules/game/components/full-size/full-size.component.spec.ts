import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FullSizeComponent } from './full-size.component';

describe('FullSizeComponent', () => {
  let component: FullSizeComponent;
  let fixture: ComponentFixture<FullSizeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FullSizeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FullSizeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
