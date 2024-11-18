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
import { DrrTextareaComponent } from '../../../shared/controls/drr-textarea/drr-textarea.component';
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
    @if (attachmentForm?.get('id')?.value) {
    <div class="attachment">
      <div class="attachment__label">
        <mat-label>{{ label ?? attachmentForm?.get('name')?.value }}</mat-label>
        <div class="attachment__label__actions">
          <button mat-stroked-button color="primary" (click)="onDownloadFile()">
            {{ t('download') }}
          </button>
          <button mat-stroked-button color="warn" (click)="onRemoveFile()">
            {{ t('delete') }}
          </button>
        </div>
      </div>
      <div class="attachment__comments">
        <drr-textarea
          [label]="t('comments')"
          [rxFormControl]="attachmentForm?.get('comments')"
          [maxlength]="2000"
        ></drr-textarea>
      </div>
    </div>
    } @else {
    <mat-hint class="required" *ngIf="isRequired()">{{
      t('required')
    }}</mat-hint>
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
        flex-direction: column;
        gap: 1rem;

        &__label {
          display: flex;
          flex-direction: row;
          justify-content: space-between;
          align-items: center;
          gap: 1rem;

          &__actions {
            display: flex;
            flex-direction: row;
            gap: 1rem;
          }
        }

        &__comments {
          display: flex;
          flex-direction: column;
          gap: 1rem;
        }
      }

      .required {
        color: red;
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
    DrrTextareaComponent,
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

  isRequired() {
    return this.attachmentForm?.invalid && this.attachmentForm?.touched;
  }
}
