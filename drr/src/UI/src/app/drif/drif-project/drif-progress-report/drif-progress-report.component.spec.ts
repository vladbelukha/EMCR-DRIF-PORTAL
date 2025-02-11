import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifProgressReportViewComponent } from './drif-progress-report.component';

describe('DrifProgressReportViewComponent', () => {
  let component: DrifProgressReportViewComponent;
  let fixture: ComponentFixture<DrifProgressReportViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifProgressReportViewComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DrifProgressReportViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
