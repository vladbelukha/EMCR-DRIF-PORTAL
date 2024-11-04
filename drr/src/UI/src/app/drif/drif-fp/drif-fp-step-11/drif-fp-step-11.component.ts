import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { FormArray, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import {
  IFormGroup,
  RxFormBuilder,
  RxFormGroup,
} from '@rxweb/reactive-form-validators';
import { AttachmentService } from '../../../../api/attachment/attachment.service';
import { DocumentType } from '../../../../model';
import { DrrFileUploadComponent } from '../../../shared/controls/drr-file-upload/drr-file-upload.component';
import { DrrInputComponent } from '../../../shared/controls/drr-input/drr-input.component';
import { DrrRadioButtonComponent } from '../../../shared/controls/drr-radio-button/drr-radio-button.component';
import { FileService } from '../../../shared/services/file.service';
import { AttachmentForm, AttachmentsForm } from '../drif-fp-form';
import {
  DrrAttahcmentComponent,
  FileUploadEvent,
} from './drif-fp-attachment.component';

@UntilDestroy({ checkProperties: true })
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
    MatButtonModule,
    DrrInputComponent,
    DrrFileUploadComponent,
    DrrRadioButtonComponent,
    DrrAttahcmentComponent,
  ],
  templateUrl: './drif-fp-step-11.component.html',
  styleUrl: './drif-fp-step-11.component.scss',
})
export class DrifFpStep11Component {
  formBuilder = inject(RxFormBuilder);
  attachmentsService = inject(AttachmentService);
  fileService = inject(FileService);

  @Input() attachmentsForm!: IFormGroup<AttachmentsForm>;

  @Input() applicationId!: string;

  async uploadFiles(event: FileUploadEvent) {
    event.files.forEach(async (file) => {
      if (file == null) {
        // TODO: show error
        return;
      }

      const base64Content = await this.fileToBase64(file);

      this.attachmentsService
        .attachmentUploadAttachment({
          applicationId: this.applicationId,
          documentType: event.documentType,
          name: file.name,
          contentType: file.type,
          content: base64Content.split(',')[1],
        })
        .subscribe({
          next: (attachment) => {
            const projectPlanFormData = {
              name: file.name,
              comments: '',
              id: attachment.id,
              documentType: event.documentType,
            } as AttachmentForm;

            const attachmentsArray = this.attachmentsForm.get(
              'attachments'
            ) as FormArray;

            const mathcingAttachment = attachmentsArray.controls.find(
              (control) => control.value.documentType === event.documentType
            );
            if (mathcingAttachment) {
              mathcingAttachment.patchValue(projectPlanFormData);
            } else {
              const fileForm = this.formBuilder.formGroup(
                AttachmentForm,
                projectPlanFormData
              ) as RxFormGroup;
              attachmentsArray.push(fileForm);
            }
          },
          error: () => {
            // TODO: show error
          },
        });
    });
  }

  fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = (error) => reject(error);
    });
  }

  uploadOtherFiles(event: File[]) {
    this.uploadFiles({
      files: event,
      documentType: 'OtherSupportingDocument',
    });
  }

  removeFile(fileId: string) {
    this.attachmentsService
      .attachmentDeleteAttachment(fileId, {
        applicationId: this.applicationId,
        id: fileId,
      })
      .subscribe({
        next: () => {
          const attachmentsArray = this.attachmentsForm.get(
            'attachments'
          ) as FormArray;
          const fileIndex = attachmentsArray.controls.findIndex(
            (control) => control.value.id === fileId
          );

          attachmentsArray.removeAt(fileIndex);
        },
        error: () => {
          // TODO: show error
        },
      });
  }

  downloadFile(fileId: string) {
    this.fileService.downloadFile(fileId);
  }

  getFormByDocumentType(documentType: DocumentType) {
    const attachmentsArray = this.attachmentsForm.get(
      'attachments'
    ) as FormArray;

    return attachmentsArray.controls.find(
      (control) => control.value.documentType === documentType
    ) as IFormGroup<AttachmentForm>;
  }

  getOtherFormArray() {
    const attachmentsArray = this.attachmentsForm.get(
      'attachments'
    ) as FormArray;

    return attachmentsArray.controls.filter(
      (control) =>
        control.value.documentType === DocumentType.OtherSupportingDocument
    );
  }

  getOtherDocumentFormGroup(index: number) {
    const otherForms = this.getOtherFormArray();
    return otherForms[index] as IFormGroup<AttachmentForm>;
  }
}
