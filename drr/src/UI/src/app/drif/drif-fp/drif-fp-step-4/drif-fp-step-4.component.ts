import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { ProjectPlanForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-4',
  standalone: true,
  imports: [],
  templateUrl: './drif-fp-step-4.component.html',
  styleUrl: './drif-fp-step-4.component.scss',
})
export class DrifFpStep4Component {
  @Input() projectPlanForm!: IFormGroup<ProjectPlanForm>;
}
