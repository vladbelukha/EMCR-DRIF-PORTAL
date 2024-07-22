import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { TranslocoModule } from '@ngneat/transloco';
import { NgxFileDropEntry, NgxFileDropModule } from 'ngx-file-drop';

@Component({
  selector: 'drr-file-upload',
  standalone: true,
  imports: [CommonModule, MatButtonModule, NgxFileDropModule, TranslocoModule],
  templateUrl: './drr-file-upload.component.html',
  styleUrl: './drr-file-upload.component.scss',
})
export class DrrFileUploadComponent {
  @Output()
  files: EventEmitter<NgxFileDropEntry[]> = new EventEmitter<
    NgxFileDropEntry[]
  >();

  dropped(files: NgxFileDropEntry[]) {
    this.files.emit(files);
  }
}
