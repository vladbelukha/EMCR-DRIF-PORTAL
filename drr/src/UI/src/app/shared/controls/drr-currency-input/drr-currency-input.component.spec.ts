import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrrCurrencyInputComponent } from './drr-currency-input.component';

describe('DrrCurrencyInputComponent', () => {
  let component: DrrCurrencyInputComponent;
  let fixture: ComponentFixture<DrrCurrencyInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrrCurrencyInputComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrrCurrencyInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
