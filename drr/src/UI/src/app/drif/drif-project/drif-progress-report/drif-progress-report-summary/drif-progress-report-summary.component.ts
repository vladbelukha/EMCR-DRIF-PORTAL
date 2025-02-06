import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { FileService } from '../../../../shared/services/file.service';
import { SummaryItemComponent } from '../../../summary-item/summary-item.component';
import { ProgressReportForm } from '../drif-progress-report-form';

@Component({
  selector: 'drif-progress-report-summary',
  standalone: true,
  imports: [
    CommonModule,
    SummaryItemComponent,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TranslocoModule,
  ],
  templateUrl: './drif-progress-report-summary.component.html',
  styleUrl: './drif-progress-report-summary.component.scss',
})
export class DrifProgressReportSummaryComponent {
  translocoService = inject(TranslocoService);
  formBuilder = inject(RxFormBuilder);
  fileService = inject(FileService);

  @Input() progressReportForm!: IFormGroup<ProgressReportForm>;
  @Input() isReadOnlyView = true;
}
