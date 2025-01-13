import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DrifProjectComponent } from './drif-project.component';

describe('DrifProjectComponent', () => {
  let component: DrifProjectComponent;
  let fixture: ComponentFixture<DrifProjectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DrifProjectComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DrifProjectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
