import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifInterimReportComponent } from './drif-interim-report.component';

describe('DrifInterimReportComponent', () => {
  let component: DrifInterimReportComponent;
  let fixture: ComponentFixture<DrifInterimReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifInterimReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifInterimReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
