import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradingHouseComponent } from './trading-house.component';

describe('TradingHouseComponent', () => {
  let component: TradingHouseComponent;
  let fixture: ComponentFixture<TradingHouseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TradingHouseComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TradingHouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
