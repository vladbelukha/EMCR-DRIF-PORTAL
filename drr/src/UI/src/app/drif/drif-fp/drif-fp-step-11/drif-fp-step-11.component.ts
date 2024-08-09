import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { DrrFileUploadComponent } from '../../../shared/controls/drr-file-upload/drr-file-upload.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { FileForm } from '../drif-fp-form';

@Component({
  selector: 'drif-fp-step-11',
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    DrrInputComponent,
    DrrFileUploadComponent,
  ],
  templateUrl: './drif-fp-step-11.component.html',
  styleUrl: './drif-fp-step-11.component.scss',
})
export class DrifFpStep11Component {
  files: FileForm[] = [];

  filesSelected(files: FileForm[]) {
    this.files.push(...files);
  }

  removeFile(index: number) {
    this.files.splice(index, 1);
  }
}
