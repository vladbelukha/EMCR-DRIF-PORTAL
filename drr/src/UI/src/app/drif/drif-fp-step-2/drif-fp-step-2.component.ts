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
import { DrrFileUploadComponent } from '../../shared/controls/drr-file-upload/drr-file-upload.component';
import { DrrInputComponent } from '../../shared/controls/drr-input/drr-input.component';
import { DrrTextareaComponent } from '../../shared/controls/drr-textarea/drr-textarea.component';
import { OwnershipAndAuthorizationForm } from '../drif-fp/drif-fp-form';

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
    const ownershipComments =
      this.ownershipAndAuthorizationForm.get('ownershipComments');
    this.ownershipAndAuthorizationForm
      .get('ownership')!
      .valueChanges.subscribe((value) => {
        if (!value) {
          ownershipComments?.addValidators(Validators.required);
        } else {
          ownershipComments?.clearValidators();
        }

        ownershipComments?.reset();
        ownershipComments?.updateValueAndValidity();
      });
  }
}
