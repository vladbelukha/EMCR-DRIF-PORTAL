import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { HotToastService } from '@ngxpert/hot-toast';
import { NgxFileDropEntry, NgxFileDropModule } from 'ngx-file-drop';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drr-file-upload',
  standalone: true,
  imports: [CommonModule, MatButtonModule, NgxFileDropModule, TranslocoModule],
  providers: [HotToastService],
  templateUrl: './drr-file-upload.component.html',
  styleUrl: './drr-file-upload.component.scss',
})
export class DrrFileUploadComponent {
  hotToast = inject(HotToastService);

  @Input()
  multiple = true;

  @Input()
  useDropzone = true;

  @Output()
  filesSelected: EventEmitter<File[]> = new EventEmitter<File[]>();

  allowedExtensions = [
    'doc',
    'docx',
    'xls',
    'xlsx',
    'ppt',
    'pttx',
    'pdf',
    'txt',
    'csv',
    'kml',
    'kmz',
    'shp',
    'xml',
    'las',
    'laz',
    'png',
    'jpg',
    'jpeg',
    'mp4',
    'avi',
    'mov',
  ];
  allowedExtensionsString = this.allowedExtensions.join(', ');

  filesDropped(files: NgxFileDropEntry[]) {
    const filesToEmit: File[] = [];
    files.map((file) => {
      const fileEntry = file.fileEntry as FileSystemFileEntry;
      fileEntry.file((file: File) => {
        filesToEmit.push(file);
      });
    });

    this.onFilesSelected(filesToEmit);
  }

  filesSelectedFromInput(event: any) {
    const filesToEmit: File[] = [];
    [...event.target.files].map((file: File) => {
      filesToEmit.push(file);
    });

    this.onFilesSelected(filesToEmit);
  }

  onFilesSelected(files: File[]) {
    const validFiles: File[] = [];
    files.forEach((file) => {
      if (file.size > 262144000) {
        this.hotToast.error(
          `Please review your files. File ${file.name} size exceeds 250MB`
        );
        return;
      }

      const fileExtension = file.name.split('.').pop();
      if (!this.allowedExtensions.includes(fileExtension!)) {
        this.hotToast.error(
          `Please review your files. File type .${fileExtension} is not supported`
        );
        return;
      }

      validFiles.push(file);
    });

    if (validFiles.length > 0) {
      this.filesSelected.emit(validFiles);
    }
  }
}
