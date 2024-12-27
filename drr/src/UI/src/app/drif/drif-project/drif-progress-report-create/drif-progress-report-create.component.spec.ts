import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifProgressReportCreateComponent } from './drif-progress-report-create.component';

describe('DrifProgressReportCreateComponent', () => {
  let component: DrifProgressReportCreateComponent;
  let fixture: ComponentFixture<DrifProgressReportCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifProgressReportCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifProgressReportCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
