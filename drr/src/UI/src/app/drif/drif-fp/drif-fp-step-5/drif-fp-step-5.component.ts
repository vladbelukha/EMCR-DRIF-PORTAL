import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { ProjectEngagementForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-5',
  standalone: true,
  imports: [],
  templateUrl: './drif-fp-step-5.component.html',
  styleUrl: './drif-fp-step-5.component.scss',
})
export class DrifFpStep5Component {
  @Input() projectEngagementForm!: IFormGroup<ProjectEngagementForm>;
}
