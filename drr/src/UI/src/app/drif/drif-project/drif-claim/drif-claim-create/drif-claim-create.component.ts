import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormArray } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { DrrCurrencyInputComponent } from '../../../../shared/controls/drr-currency-input/drr-currency-input.component';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import { DrrInputComponent } from '../../../../shared/controls/drr-input/drr-input.component';
import { DrrRadioButtonComponent } from '../../../../shared/controls/drr-radio-button/drr-radio-button.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';
import { DrrTextareaComponent } from '../../../../shared/controls/drr-textarea/drr-textarea.component';
import { ClaimForm, InvoiceForm } from '../drif-claim-form';

@Component({
  selector: 'drr-drif-claim-create',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    TranslocoModule,
    RouterModule,
    DrrDatepickerComponent,
    DrrInputComponent,
    DrrSelectComponent,
    DrrRadioButtonComponent,
    DrrTextareaComponent,
    DrrCurrencyInputComponent,
  ],
  templateUrl: './drif-claim-create.component.html',
  styleUrl: './drif-claim-create.component.scss',
  providers: [RxFormBuilder],
})
export class DrifClaimCreateComponent {
  formBuilder = inject(RxFormBuilder);
  route = inject(ActivatedRoute);
  router = inject(Router);

  projectId?: string;

  claimForm = this.formBuilder.formGroup(ClaimForm) as IFormGroup<ClaimForm>;

  claimCategoryOptions: DrrSelectOption[] = [
    { label: 'Option 1', value: 'option1' },
    { label: 'Option 2', value: 'option2' },
  ];

  getInvoiceFormArray() {
    return this.claimForm.get('invoices') as FormArray;
  }

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
    });

    // TODO: temp init array
    this.getInvoiceFormArray().push(this.formBuilder.formGroup(InvoiceForm));
    this.getInvoiceFormArray().push(this.formBuilder.formGroup(InvoiceForm));
  }

  stepperOrientation: StepperOrientation = 'horizontal';

  stepperSelectionChange(event: any) {}

  save() {}

  goBack() {
    // TODO: save

    this.router.navigate(['drif-projects', this.projectId]);
  }

  addInvoice() {
    this.getInvoiceFormArray().push(this.formBuilder.formGroup(InvoiceForm));
  }

  removeInvoice(index: number) {
    this.getInvoiceFormArray().removeAt(index);
  }
}
