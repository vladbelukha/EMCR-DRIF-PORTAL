import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrrAlertComponent } from './drr-alert.component';

describe('DrrAlertComponent', () => {
  let component: DrrAlertComponent;
  let fixture: ComponentFixture<DrrAlertComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrrAlertComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrrAlertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
