import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifForecastComponent } from './drif-forecast.component';

describe('DrifForecastComponent', () => {
  let component: DrifForecastComponent;
  let fixture: ComponentFixture<DrifForecastComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifForecastComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifForecastComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
