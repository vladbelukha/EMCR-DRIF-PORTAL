import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EOIApplicationComponent } from './eoi-application.component';

describe('StartApplicationComponent', () => {
  let component: EOIApplicationComponent;
  let fixture: ComponentFixture<EOIApplicationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EOIApplicationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EOIApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
