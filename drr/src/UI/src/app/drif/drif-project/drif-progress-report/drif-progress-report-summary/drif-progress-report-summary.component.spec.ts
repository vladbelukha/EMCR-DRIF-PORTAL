import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifProgressReportSummaryComponent } from './drif-progress-report-summary.component';

describe('DrifProgressReportSummaryComponent', () => {
  let component: DrifProgressReportSummaryComponent;
  let fixture: ComponentFixture<DrifProgressReportSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifProgressReportSummaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifProgressReportSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
