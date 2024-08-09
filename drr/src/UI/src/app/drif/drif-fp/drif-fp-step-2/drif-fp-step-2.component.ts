import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
import { OwnershipAndAuthorizationForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-2',
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    DrrTextareaComponent,

    DrrInputComponent,
  ],
  templateUrl: './drif-fp-step-2.component.html',
  styleUrl: './drif-fp-step-2.component.scss',
})
export class DrifFpStep2Component {
  @Input()
  ownershipAndAuthorizationForm!: IFormGroup<OwnershipAndAuthorizationForm>;

  ngOnInit() {
    const ownershipDescription = this.ownershipAndAuthorizationForm.get(
      'ownershipDescription'
    );

    // check if comments should be mandatory
    if (
      !this.ownershipAndAuthorizationForm.get('ownershipDeclaration')!.value
    ) {
      ownershipDescription?.addValidators(Validators.required);
      ownershipDescription?.updateValueAndValidity();
    }

    this.ownershipAndAuthorizationForm
      .get('ownershipDeclaration')!
      .valueChanges.subscribe((value) => {
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
