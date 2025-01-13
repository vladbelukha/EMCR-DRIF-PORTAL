import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifProgressReportComponent } from './drif-progress-report.component';

describe('DrifProgressReportComponent', () => {
  let component: DrifProgressReportComponent;
  let fixture: ComponentFixture<DrifProgressReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifProgressReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifProgressReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
