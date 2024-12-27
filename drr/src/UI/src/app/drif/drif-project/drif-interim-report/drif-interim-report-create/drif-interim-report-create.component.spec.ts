import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifInterimReportCreateComponent } from './drif-interim-report-create.component';

describe('DrifInterimReportCreateComponent', () => {
  let component: DrifInterimReportCreateComponent;
  let fixture: ComponentFixture<DrifInterimReportCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifInterimReportCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifInterimReportCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
