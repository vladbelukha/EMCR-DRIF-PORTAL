import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup, RxFormGroup } from '@rxweb/reactive-form-validators';
import { NgxMaskPipe } from 'ngx-mask';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';

@Component({
  selector: 'drr-summary',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule, NgxMaskPipe],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
  private _eoiApplicationForm?: IFormGroup<EOIApplicationForm>;

  @Input()
  set eoiApplicationForm(eoiApplicationForm: IFormGroup<EOIApplicationForm>) {
    this._eoiApplicationForm = eoiApplicationForm;
  }

  get eoiApplicationForm(): IFormGroup<EOIApplicationForm> {
    return this._eoiApplicationForm!;
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
      this._eoiApplicationForm
        ?.get('fundingInformation')
        ?.get('remainingAmount')?.value ?? 0
    );
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

  getFormArray(groupName: string, controlName: string): any[] {
    return this.getGroup(groupName)?.get(controlName)?.value ?? [];
  }
}
