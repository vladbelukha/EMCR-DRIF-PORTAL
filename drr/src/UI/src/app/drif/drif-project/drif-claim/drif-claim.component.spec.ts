import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifClaimComponent } from './drif-claim.component';

describe('DrifClaimComponent', () => {
  let component: DrifClaimComponent;
  let fixture: ComponentFixture<DrifClaimComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifClaimComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifClaimComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
