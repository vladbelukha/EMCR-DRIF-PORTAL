import { inject, Injectable } from '@angular/core';
import { AttachmentService } from '../../..';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  private attachmentsService = inject(AttachmentService);

  downloadFile(fileId: string) {
    this.attachmentsService.attachmentDownloadAttachment(fileId).subscribe({
      next: (response) => {
        if (response.file?.content == null) {
          // TODO: show error
          return;
        }

        // alternative way to download file
        // const byteArray = this.base64ToByteArray(response.file.content!);
        // const blob = new Blob([byteArray], { type: response.file.contentType });
        // saveAs(blob, response.file.fileName);

        const byteArray = this.base64ToByteArray(response.file.content!);
        const blob = new Blob([byteArray], {
          type: response.file.contentType,
        });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = response.file.fileName!;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => {},
    });
  }

  private base64ToByteArray(base64: string): Uint8Array {
    const binaryString = window.atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }
}
