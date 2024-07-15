import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifFpComponent } from './drif-fp.component';

describe('DrifFpComponent', () => {
  let component: DrifFpComponent;
  let fixture: ComponentFixture<DrifFpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifFpComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DrifFpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
