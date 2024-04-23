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
  RxFormControl,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { TranslocoModule } from '@ngneat/transloco';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { DrrInputComponent } from '../drr-input/drr-input.component';
import { Subscription } from 'rxjs';

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
    DrrInputComponent,
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

  getFormControl(name: string): RxFormControl {
    return this.eoiApplicationForm.get(name) as RxFormControl;
  }

  getGroupFormControl(controlName: string, groupName: string): RxFormControl {
    return this.eoiApplicationForm
      .get(groupName)
      ?.get(controlName) as RxFormControl;
  }

  getArrayFormControl(
    controlName: string,
    arrayName: string,
    index: number
  ): RxFormControl {
    return this.getFormArray(arrayName)?.controls[index]?.get(
      controlName
    ) as RxFormControl;
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

    let submitterSub: Subscription | undefined;

    if (sameAsSubmitter) {
      const submitter = this.eoiApplicationForm.get('submitter')?.value;
      projectContact.patchValue(submitter);
      submitterSub = this.eoiApplicationForm
        .get('submitter')
        ?.valueChanges.subscribe((submitter) => {
          projectContact.patchValue(submitter);
        });
    } else {
      projectContact.reset();
      submitterSub?.unsubscribe();
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
