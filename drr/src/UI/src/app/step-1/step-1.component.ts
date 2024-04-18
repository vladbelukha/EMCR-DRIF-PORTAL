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
  StringItem,
} from '../eoi-application/eoi-application-form';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { TranslocoModule } from '@ngneat/transloco';
import { MatCheckboxModule } from '@angular/material/checkbox';

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
    MatCheckboxModule,
    TranslocoModule,
  ],
  templateUrl: './step-1.component.html',
  styleUrl: './step-1.component.scss',
})
export class Step1Component {
  formBuilder = inject(RxFormBuilder);

  @Input()
  eoiApplicationForm!: IFormGroup<EOIApplicationForm>;

  ngOnInit() {
    this.eoiApplicationForm
      .get('partneringProponentsArray')
      ?.valueChanges.subscribe((proponents: StringItem[]) => {
        this.eoiApplicationForm
          .get('partneringProponents')
          ?.patchValue(proponents.map((proponent) => proponent.value));
      });
  }

  getFormArray(formArrayName: string) {
    return this.eoiApplicationForm.get(formArrayName) as FormArray;
  }

  addAdditionalContact() {
    this.getFormArray('additionalContacts').push(
      this.formBuilder.formGroup(ContactDetailsForm)
    );
  }

  removeAdditionalContact(index: number) {
    const additionalContacts = this.getFormArray('additionalContacts');
    additionalContacts.removeAt(index);
  }

  toggleSameAsSubmitter() {
    const sameAsSubmitter =
      this.eoiApplicationForm.get('sameAsSubmitter')?.value;

    const projectContact = this.eoiApplicationForm.get(
      'projectContact'
    ) as RxFormGroup;

    if (sameAsSubmitter) {
      const submitter = this.eoiApplicationForm.get('submitter')?.value;
      projectContact.patchValue(submitter);
    } else {
      projectContact.reset();
    }
  }

  addProponent() {
    const proponents = this.getFormArray('partneringProponentsArray');
    proponents.push(this.formBuilder.formGroup(StringItem));
  }

  removeProponent(index: number) {
    const proponents = this.getFormArray('partneringProponentsArray');
    proponents.removeAt(index);
  }
}
