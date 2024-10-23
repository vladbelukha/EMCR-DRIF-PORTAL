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

  @Input() attachmentsForm!: IFormGroup<AttachmentsForm>;

  @Input() applicationId!: string;

  uploadFiles(event: FileUploadEvent) {
    // TODO: change to foreach file
    const file = event.files[0];

    // TODO: probably do some size verification here

    if (file == null) {
      // TODO: show error
      return;
    }

    this.attachmentsService
      .attachmentUploadAttachment({
        // TODO: probably use FileData to match FileInfo model
        applicationId: this.applicationId,
        documentType: event.documentType,
        name: file.name,
        comments: '',
      })
      .subscribe({
        next: (attachment) => {
          const projectPlanFormData = {
            name: file.name,
            comments: '',
            id: attachment.id,
            documentType: event.documentType,
          } as AttachmentForm;

          const fileForm = this.formBuilder.formGroup(
            AttachmentForm,
            projectPlanFormData
          ) as RxFormGroup;

          const attachmentsArray = this.attachmentsForm.get(
            'attachments'
          ) as FormArray;
          attachmentsArray.push(fileForm);
        },
        error: () => {
          // TODO: show error
        },
      });
  }

  uploadOtherFiles(event: File[]) {
    // this.uploadFiles({
    //   files: event,
    //   // TODO: documentType: 'Other',
    // });
  }

  removeFile(fileId: string) {
    this.attachmentsService
      .attachmentDeleteAttachment(fileId, {
        // TODO: this body is not needed
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
      (control) => control.value.documentType === 'DocumentType.Other' // TODO: use enum value when available
    );
  }
}
