import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { DocumentType } from '../../../../model';
import { DrrFileUploadComponent } from '../../../shared/controls/drr-file-upload/drr-file-upload.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { AttachmentForm } from '../drif-fp-form';

export interface FileUploadEvent {
  files: File[];
  documentType: DocumentType;
}

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drr-attachment',
  template: ` <div
    class="attachment-container"
    *transloco="let t; read: 'attachments'"
  >
    <mat-label>{{ label }}</mat-label>
    @if (attachmentForm) {
    <div>
      <button mat-raised-button color="primary" (click)="onDownloadFile()">
        {{ t('downloadFile') }}
      </button>
    </div>
    <div class="attachment">
      <drr-input
        class="drr-single-input"
        [label]="t('name')"
        [rxFormControl]="attachmentForm.get('name')"
      ></drr-input>
      <drr-input
        class="drr-single-input"
        [label]="t('comments')"
        [rxFormControl]="attachmentForm.get('comments')"
      ></drr-input>
      <button mat-mini-fab color="warn" (click)="onRemoveFile()">
        <mat-icon>delete</mat-icon>
      </button>
    </div>
    } @else {
    <drr-file-upload
      (filesSelected)="onUploadFiles($event)"
      [multiple]="false"
    ></drr-file-upload>
    }
  </div>`,
  styles: [
    `
      .attachment-container {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .attachment {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: baseline;
        gap: 1rem;
      }
    `,
  ],
  standalone: true,
  imports: [
    CommonModule,
    TranslocoModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    DrrInputComponent,
    DrrFileUploadComponent,
  ],
})
export class DrrAttahcmentComponent {
  @Input() label?: string;

  @Input() attachmentForm?: IFormGroup<AttachmentForm>;

  @Input() documentType?: DocumentType;

  @Output()
  uploadFiles: EventEmitter<FileUploadEvent> =
    new EventEmitter<FileUploadEvent>();

  @Output()
  removeFile: EventEmitter<string> = new EventEmitter<string>();

  @Output()
  downloadFile: EventEmitter<string> = new EventEmitter<string>();

  onUploadFiles(files: File[]) {
    this.uploadFiles.emit({
      files,
      documentType: this.documentType!,
    });
  }

  onRemoveFile() {
    this.removeFile.emit(this.attachmentForm?.get('id')?.value);
  }

  onDownloadFile() {
    this.downloadFile.emit(this.attachmentForm?.get('id')?.value);
  }
}
