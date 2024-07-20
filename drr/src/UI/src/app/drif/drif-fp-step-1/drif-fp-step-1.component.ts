import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DrifEoiStep1Component } from '../drif-eoi-step-1/drif-eoi-step-1.component';
import { ProponentInformationForm } from '../drif-eoi/drif-eoi-form';

@Component({
  selector: 'drif-fp-step-1',
  standalone: true,
  imports: [DrifEoiStep1Component, MatInputModule, MatIconModule],
  templateUrl: './drif-fp-step-1.component.html',
  styleUrl: './drif-fp-step-1.component.scss',
})
export class DrifFpStep1Component {
  @Input()
  proponentInformationForm!: IFormGroup<ProponentInformationForm>;
}
