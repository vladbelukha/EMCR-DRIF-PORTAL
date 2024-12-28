import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifClaimCreateComponent } from './drif-claim-create.component';

describe('DrifClaimCreateComponent', () => {
  let component: DrifClaimCreateComponent;
  let fixture: ComponentFixture<DrifClaimCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifClaimCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifClaimCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
