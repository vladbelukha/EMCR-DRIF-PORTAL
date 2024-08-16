import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrrFundingListComponent } from './drr-funding-list.component';

describe('DrrFundingListComponent', () => {
  let component: DrrFundingListComponent;
  let fixture: ComponentFixture<DrrFundingListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrrFundingListComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrrFundingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
