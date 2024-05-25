import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormGroup } from '@rxweb/reactive-form-validators';
import { NgxMaskPipe } from 'ngx-mask';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';
import { SummaryItemComponent } from '../summary-item/summary-item.component';

@Component({
  selector: 'drr-summary',
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    TranslocoModule,
    NgxMaskPipe,
    SummaryItemComponent,
  ],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
  private _eoiApplicationForm?: IFormGroup<EOIApplicationForm>;
  private _eoiApplication?: EOIApplicationForm;

  @Input()
  set eoiApplicationForm(eoiApplicationForm: IFormGroup<EOIApplicationForm>) {
    this._eoiApplicationForm = eoiApplicationForm;
    this._eoiApplication = eoiApplicationForm.getRawValue();
  }

  get eoiApplicationForm(): IFormGroup<EOIApplicationForm> {
    return this._eoiApplicationForm!;
  }

  get eoiApplication(): EOIApplicationForm {
    return this._eoiApplication!;
  }

  objectKeys(obj: any) {
    const excludeKeys = [
      'sameAsSubmitter',
      'partneringProponentsArray',
      'infrastructureImpactedArray',
      'recaptcha',
      'declaration',
    ];
    return Object.keys(obj?.controls).filter(
      (key) => !excludeKeys.includes(key)
    );
  }

  objectValues(obj: any) {
    return Object.values(obj?.controls);
  }

  getGroup(groupName: string): RxFormGroup {
    return this.eoiApplicationForm?.get(groupName) as RxFormGroup;
  }

  getGroupControl(groupName: string, controlName: string) {
    return this.getGroup(groupName)?.get(controlName);
  }

  getRemainingAmountAbs() {
    return Math.abs(
      this.eoiApplication?.fundingInformation?.remainingAmount ?? 0
    );
  }
}
