import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { ProjectOutcomesForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-8',
  standalone: true,
  imports: [],
  templateUrl: './drif-fp-step-8.component.html',
  styleUrl: './drif-fp-step-8.component.scss',
})
export class DrifFpStep8Component {
  @Input() projectOutcomesForm!: IFormGroup<ProjectOutcomesForm>;
}
