import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CargoShipComponent } from './cargo-ship.component';

describe('CargoShipComponent', () => {
  let component: CargoShipComponent;
  let fixture: ComponentFixture<CargoShipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CargoShipComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CargoShipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
