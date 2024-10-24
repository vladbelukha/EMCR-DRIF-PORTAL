import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { NgxFileDropEntry, NgxFileDropModule } from 'ngx-file-drop';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drr-file-upload',
  standalone: true,
  imports: [CommonModule, MatButtonModule, NgxFileDropModule, TranslocoModule],
  templateUrl: './drr-file-upload.component.html',
  styleUrl: './drr-file-upload.component.scss',
})
export class DrrFileUploadComponent {
  @Input()
  multiple = true;

  @Input()
  useDropzone = true;

  @Output()
  filesSelected: EventEmitter<File[]> = new EventEmitter<File[]>();

  filesDropped(files: NgxFileDropEntry[]) {
    const filesToEmit: File[] = [];
    files.map((file) => {
      const fileEntry = file.fileEntry as FileSystemFileEntry;
      fileEntry.file((file: File) => {
        filesToEmit.push(file);
      });
    });
    this.filesSelected.emit(filesToEmit);
  }

  filesSelectedFromInput(event: any) {
    const filesToEmit: File[] = [];
    [...event.target.files].map((file: File) => {
      filesToEmit.push(file);
    });

    this.filesSelected.emit(filesToEmit);
  }
}
