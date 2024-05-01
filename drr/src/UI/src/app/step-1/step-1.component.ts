import { CommonModule } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { TranslocoModule } from '@ngneat/transloco';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormControl,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { Subscription } from 'rxjs';
import { DrrInputComponent } from '../drr-input/drr-input.component';
import {
  ContactDetailsForm,
  ProponentInformationForm,
  StringItem,
} from '../eoi-application/eoi-application-form';

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

  submitterSub: Subscription | undefined;

  @Input()
  proponentInformationForm!: IFormGroup<ProponentInformationForm>;

  ngOnInit() {
    this.proponentInformationForm
      .get('partneringProponentsArray')
      ?.valueChanges.subscribe((proponents: StringItem[]) => {
        this.proponentInformationForm
          .get('partneringProponents')
          ?.patchValue(proponents.map((proponent) => proponent.value));
      });
  }

  getFormArray(formArrayName: string) {
    return this.proponentInformationForm.get(formArrayName) as FormArray;
  }

  getFormControl(name: string): RxFormControl {
    return this.proponentInformationForm.get(name) as RxFormControl;
  }

  getGroupFormControl(controlName: string, groupName: string): RxFormControl {
    return this.proponentInformationForm
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
      this.proponentInformationForm.get('sameAsSubmitter')?.value;

    const projectContact = this.proponentInformationForm.get(
      'projectContact'
    ) as RxFormGroup;

    if (sameAsSubmitter) {
      const submitter = this.proponentInformationForm.get('submitter')?.value;
      projectContact.disable();
      projectContact.patchValue(submitter);
      this.submitterSub = this.proponentInformationForm
        .get('submitter')
        ?.valueChanges.subscribe((submitter) => {
          projectContact.patchValue(submitter);
        });
    } else {
      projectContact.reset();
      projectContact.enable();
      this.submitterSub?.unsubscribe();
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
