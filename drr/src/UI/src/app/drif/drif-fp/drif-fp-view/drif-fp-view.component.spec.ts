import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifFpViewComponent } from './drif-fp-view.component';

describe('DrifFpViewComponent', () => {
  let component: DrifFpViewComponent;
  let fixture: ComponentFixture<DrifFpViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifFpViewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrifFpViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
