import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  Input,
  inject,
} from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import {
  ContactDetailsForm,
  EOIApplicationForm,
} from '../eoi-application/eoi-application-form';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'drr-step-1',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatIconModule,
    MatDividerModule,
    MatButtonModule,
  ],
  templateUrl: './step-1.component.html',
  styleUrl: './step-1.component.scss',
})
export class Step1Component {
  formBuilder = inject(RxFormBuilder);

  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  addProjectContact() {
    this.getFormArray('projectContacts').push(
      this.formBuilder.formGroup(ContactDetailsForm),
      { emitEvent: false }
    ); // TODO: still makes parent dirty, which is not ideal
  }

  removeProjectContact(index: number) {
    const projectContacts = this.getFormArray('projectContacts').controls;
    projectContacts.splice(index, 1);
  }
}
