import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { distinctUntilChanged } from 'rxjs';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { LocationInformationForm } from '../drif-eoi-form';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drif-eoi-step-4',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    TranslocoModule,
    DrrTextareaComponent,
    DrrRadioButtonComponent,
  ],
  templateUrl: './drif-eoi-step-4.component.html',
  styleUrl: './drif-eoi-step-4.component.scss',
})
export class DrifEoiStep4Component {
  @Input()
  locationInformationForm!: IFormGroup<LocationInformationForm>;

  ngOnInit() {
    const ownershipDescription = this.locationInformationForm.get(
      'ownershipDescription'
    );
    this.locationInformationForm
      .get('ownershipDeclaration')!
      .valueChanges.pipe(distinctUntilChanged())
      .subscribe((value) => {
        if (!value) {
          ownershipDescription?.addValidators(Validators.required);
        } else {
          ownershipDescription?.clearValidators();
        }

        ownershipDescription?.reset();
        ownershipDescription?.updateValueAndValidity();
      });
  }
}
