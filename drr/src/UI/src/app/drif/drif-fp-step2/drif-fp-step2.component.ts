import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { FileForm, ProponentEligibilityForm } from '../drif-fp/drif-fp-form';

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
    DrrFileUploadComponent,
    DrrInputComponent,
  ],
  templateUrl: './drif-fp-step2.component.html',
  styleUrl: './drif-fp-step2.component.scss',
})
export class DrifFpStep2Component {
  @Input()
  proponentEligibilityForm!: IFormGroup<ProponentEligibilityForm>;

  files: FileForm[] = [];

  filesSelected(files: FileForm[]) {
    this.files.push(...files);
  }

  removeFile(index: number) {
    this.files.splice(index, 1);
  }
}
