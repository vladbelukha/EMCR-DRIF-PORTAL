import { Component, Input, input } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ContactDetailsForm } from '../eoi-application/eoi-application-form';
import { RxFormControl, RxFormGroup } from '@rxweb/reactive-form-validators';
import { MatInputModule } from '@angular/material/input';
import { DrrInputComponent } from '../drr-input/drr-input.component';
import { TranslocoModule } from '@ngneat/transloco';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'drr-contact-details',
  standalone: true,
  imports: [
    MatInputModule,
    MatFormFieldModule,
    DrrInputComponent,
    TranslocoModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  templateUrl: './contact-details.component.html',
  styleUrl: './contact-details.component.scss',
})
export class DrrContactDetailsComponent {
  @Input()
  contactForm?: RxFormGroup;

  getFormControl(name: string): RxFormControl {
    return this.contactForm?.get(name) as RxFormControl;
  }
}
