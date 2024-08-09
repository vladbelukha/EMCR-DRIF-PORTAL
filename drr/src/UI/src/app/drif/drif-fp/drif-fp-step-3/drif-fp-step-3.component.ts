import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { ProjectAreaForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-3',
  standalone: true,
  imports: [],
  templateUrl: './drif-fp-step-3.component.html',
  styleUrl: './drif-fp-step-3.component.scss',
})
export class DrifFpStep3Component {
  @Input() projectAreaForm!: IFormGroup<ProjectAreaForm>;
}
