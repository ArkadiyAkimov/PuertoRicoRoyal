import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CargoShipsComponent } from './cargo-ships.component';

describe('CargoShipsComponent', () => {
  let component: CargoShipsComponent;
  let fixture: ComponentFixture<CargoShipsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CargoShipsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CargoShipsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
