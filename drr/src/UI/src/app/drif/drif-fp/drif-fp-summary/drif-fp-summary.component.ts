import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import {
  IFormGroup,
  RxFormArray,
  RxFormBuilder,
  RxFormControl,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { NgxMaskPipe } from 'ngx-mask';
import { DocumentType, YesNoOption } from '../../../../model';
import { DrifEoiSummaryComponent } from '../../drif-eoi/drif-eoi-summary/drif-eoi-summary.component';
import { SummaryItemComponent } from '../../summary-item/summary-item.component';
import { AttachmentForm, DrifFpForm } from '../drif-fp-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-fp-summary',
  standalone: true,
  imports: [
    CommonModule,
    SummaryItemComponent,
    MatCardModule,
    MatInputModule,
    TranslocoModule,
    NgxMaskPipe,
    DrifEoiSummaryComponent,
  ],
  templateUrl: './drif-fp-summary.component.html',
  styleUrl: './drif-fp-summary.component.scss',
})
export class DrifFpSummaryComponent {
  translocoService = inject(TranslocoService);
  private _fullProposalForm?: IFormGroup<DrifFpForm>;
  formBuilder = inject(RxFormBuilder);

  @Input()
  showSubmitterInfo = true;

  @Input()
  set fullProposalForm(DrifFpForm: IFormGroup<DrifFpForm>) {
    this._fullProposalForm = DrifFpForm;
  }
  get fullProposalForm(): IFormGroup<DrifFpForm> {
    return this._fullProposalForm!;
  }

  getGroup(groupName: string): RxFormGroup {
    return this.fullProposalForm?.get(groupName) as RxFormGroup;
  }

  getFormArray(groupName: string, controlName: string): any[] {
    return this.getGroup(groupName)?.get(controlName)?.value ?? [];
  }

  getRxFormControl(controlFullName: string) {
    return this.fullProposalForm.get(controlFullName) as RxFormControl;
  }

  getRxGroupFormControl(
    groupName: string,
    nestedGroup: string,
    controlName: string
  ) {
    return this.getGroup(groupName)
      ?.get(nestedGroup)
      ?.get(controlName) as RxFormControl;
  }

  getRxFormArrayControls(groupName: string, controlName: string) {
    return (this.getGroup(groupName)?.get(controlName) as RxFormArray).controls;
  }

  getAttachmentByDocumentType(documentType: DocumentType) {
    const attachmentsArray = this.fullProposalForm.get(
      'attachments.attachments'
    ) as RxFormArray;
    const attahcment = attachmentsArray.controls.find(
      (control) => control.get('documentType')?.value === documentType
    ) as RxFormGroup;

    return (
      attahcment ??
      // TODO: not sure if this is the correct way to handle this
      (this.formBuilder.formGroup(AttachmentForm, {}) as RxFormGroup)
    );
  }

  convertRxFormControl(formControl: AbstractControl<any, any> | null) {
    return formControl as RxFormControl;
  }

  objectHasValues(obj: any): boolean {
    // if array - check length, if value - check if truthy
    return (
      obj &&
      Object.values(obj).some((value) => {
        if (Array.isArray(value)) {
          return this.arrayHasValues(value);
        } else {
          return !!value;
        }
      })
    );
  }

  arrayHasValues(array: any[]): boolean {
    return (
      array &&
      array.length > 0 &&
      array.some((value) => this.objectHasValues(value))
    );
  }

  getRemainingAmountAbs() {
    return Math.abs(
      this.fullProposalForm?.get('budget.remainingAmount')?.value ?? 0
    );
  }

  getPreviousResponseValue() {
    return this.fullProposalForm.get('budget.previousResponse')?.value ===
      YesNoOption.NotApplicable
      ? this.translocoService.translate('costUnknown')
      : this.translocoService.translate(
          this.fullProposalForm.get('budget.previousResponse')?.value
        );
  }
}
