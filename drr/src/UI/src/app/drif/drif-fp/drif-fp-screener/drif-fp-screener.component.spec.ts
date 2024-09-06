import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifFpScreenerComponent } from './drif-fp-screener.component';

describe('DrifFpScreenerComponent', () => {
  let component: DrifFpScreenerComponent;
  let fixture: ComponentFixture<DrifFpScreenerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifFpScreenerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrifFpScreenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
