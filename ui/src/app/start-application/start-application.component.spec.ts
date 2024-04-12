import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StartApplicationComponent } from './start-application.component';

describe('StartApplicationComponent', () => {
  let component: StartApplicationComponent;
  let fixture: ComponentFixture<StartApplicationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StartApplicationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(StartApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
