import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifForecastCreateComponent } from './drif-forecast-create.component';

describe('DrifForecastCreateComponent', () => {
  let component: DrifForecastCreateComponent;
  let fixture: ComponentFixture<DrifForecastCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifForecastCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifForecastCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
