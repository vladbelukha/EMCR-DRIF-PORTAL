import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import {
  FormArray,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectChange } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';
import { RxFormBuilder } from '@rxweb/reactive-form-validators';
import { FundingType } from '../../../model';
import { DrrCurrencyInputComponent } from '../../shared/controls/drr-currency-input/drr-currency-input.component';
import { DrrInputComponent } from '../../shared/controls/drr-input/drr-input.component';
import { DrrSelectComponent } from '../../shared/controls/drr-select/drr-select.component';
import { FundingInformationItemForm } from '../drif-eoi/drif-eoi-form';

@Component({
  selector: 'drr-funding-list',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    DrrCurrencyInputComponent,
    DrrInputComponent,
    DrrSelectComponent,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './drr-funding-list.component.html',
  styleUrl: './drr-funding-list.component.scss',
})
export class DrrFundingListComponent {
  breakpointObserver = inject(BreakpointObserver);
  formBuilder = inject(RxFormBuilder);

  isMobile = false;
  fundingTypeOptions: string[] = Object.values(FundingType);

  @Input()
  fundingFormArray!: FormArray;

  ngOnInit() {
    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.isMobile = !matches;
      });
  }

  setFundingTypeDesctiption(event: MatSelectChange, index: number) {
    const descriptionControl = this.fundingFormArray
      .at(index)
      .get('otherDescription');

    // check if value contains FundingType.OtherGrants
    if (this.hasOtherGrants(event.value)) {
      descriptionControl?.addValidators(Validators.required);
    } else {
      descriptionControl?.clearValidators();
    }

    descriptionControl?.reset();
    descriptionControl?.updateValueAndValidity();
  }

  hasOtherGrants(selectValue: FundingType[]) {
    return selectValue?.includes(FundingType.OtherGrants);
  }

  addOtherFunding() {
    this.fundingFormArray.push(
      this.formBuilder.formGroup(FundingInformationItemForm)
    );
  }

  removeOtherSource(index: number) {
    this.fundingFormArray.removeAt(index);
  }
}
