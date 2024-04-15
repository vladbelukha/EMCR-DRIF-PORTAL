import { Component, Input, input } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ContactDetailsForm } from '../eoi-application/eoi-application-form';
import { RxFormGroup } from '@rxweb/reactive-form-validators';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'drr-contact-details',
  standalone: true,
  imports: [MatInputModule, MatFormFieldModule],
  templateUrl: './contact-details.component.html',
  styleUrl: './contact-details.component.scss'
})
export class ContactDetailsComponent {
  @Input()
  contactForm?: RxFormGroup;
}
