import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { DrifapplicationService } from '../../../api/drifapplication/drifapplication.service';
import { Submission } from '../../../model';

@Component({
  selector: 'drr-submission-list',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatTableModule],
  templateUrl: './submission-list.component.html',
  styleUrl: './submission-list.component.scss',
})
export class SubmissionListComponent {
  router = inject(Router);

  applicationService = inject(DrifapplicationService);

  submissionListColumns = [
    'id',
    'projectTitle',
    'status',
    'fundingRequest',
    'modifiedDate',
    'submittedDate',
    'partneringProponents',
    'actions',
  ];
  submissionListDataSource = new MatTableDataSource<Submission>();

  ngOnInit() {
    this.applicationService.dRIFApplicationGetAll().subscribe((submissions) => {
      this.submissionListDataSource = new MatTableDataSource(submissions);
    });
  }

  onCreateFormClick() {
    this.router.navigate(['/eoi-application']);
  }

  onViewFormClick(id: string) {
    this.router.navigate(['/eoi-application', id]);
  }
}
