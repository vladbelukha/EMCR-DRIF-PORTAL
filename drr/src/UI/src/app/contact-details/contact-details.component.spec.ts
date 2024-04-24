import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrrContactDetailsComponent } from './contact-details.component';

describe('ContactDetailsComponent', () => {
  let component: DrrContactDetailsComponent;
  let fixture: ComponentFixture<DrrContactDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrrContactDetailsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DrrContactDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
