import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrrSelectComponent } from './drr-select.component';

describe('DrrSelectComponent', () => {
  let component: DrrSelectComponent;
  let fixture: ComponentFixture<DrrSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrrSelectComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrrSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
