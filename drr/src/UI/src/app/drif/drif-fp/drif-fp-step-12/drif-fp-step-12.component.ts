import { Component, Input } from '@angular/core';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DeclarationsForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-12',
  standalone: true,
  imports: [],
  templateUrl: './drif-fp-step-12.component.html',
  styleUrl: './drif-fp-step-12.component.scss',
})
export class DrifFpStep12Component {
  @Input() declationsForm!: IFormGroup<DeclarationsForm>;
}
