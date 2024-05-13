import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrrDatepickerComponent } from './drr-datepicker.component';

describe('DrrDatepickerComponent', () => {
  let component: DrrDatepickerComponent;
  let fixture: ComponentFixture<DrrDatepickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrrDatepickerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrrDatepickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
