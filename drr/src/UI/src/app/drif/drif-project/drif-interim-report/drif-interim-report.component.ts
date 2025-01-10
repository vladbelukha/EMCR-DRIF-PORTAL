import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, RouterModule } from '@angular/router';
import {
  ClaimStatus,
  ForecastStatus,
  InterimReport,
  InterimReportStatus,
  ProgressReportStatus,
} from '../../../../model';

@Component({
  selector: 'drr-drif-interim-report',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatChipsModule,
    MatButtonModule,
    MatDividerModule,
    MatMenuModule,
    MatIconModule,
    MatTabsModule,
  ],
  templateUrl: './drif-interim-report.component.html',
  styleUrl: './drif-interim-report.component.scss',
})
export class DrifInterimReportComponent {
  route = inject(ActivatedRoute);

  projectId?: string;
  reportId?: string;

  interimReport?: InterimReport;

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
      this.reportId = params['reportId'];
    });

    this.interimReport = {
      id: 'IR-0001',
      dueDate: '2021-01-01',
      status: InterimReportStatus.InReview,
      description: 'Description 1',
      claim: {
        id: 'CL-0001',
        claimType: 'Claim 1',
        claimDate: '2021-01-01',
        claimAmount: 1000,
        status: ClaimStatus.InProgress,
        invoiceCount: 5,
        earliestInvoice: '2021-03-21',
        latestInvoice: '2022-11-15',
      },
      report: {
        id: 'PR-0001',
        reportType: 'Report 1',
        reportDate: '2021-01-01',
        status: ProgressReportStatus.Approved,
      },

      forecast: {
        status: ForecastStatus.Pending,
      },
      // {
      //   id: 'FC-0001',
      //   forecastType: 'Forecast 1',
      //   forecastDate: '2021-01-01',
      //   forecastAmount: 1000,
      //   status: ForecastStatus.Rejected,
      // },
    } as InterimReport;
  }

  getStatusColorClass(status?: string) {
    switch (status) {
      case InterimReportStatus.InReview:
      case ClaimStatus.InProgress:
      case ProgressReportStatus.Pending:
      case ForecastStatus.Pending:
        return 'pending';
      case InterimReportStatus.InReview:
      case ClaimStatus.InProgress:
      case ProgressReportStatus.Review:
      case ForecastStatus.Review:
        return 'review';
      case InterimReportStatus.Approved:
      case ClaimStatus.Approved:
      case ProgressReportStatus.Approved:
      case ForecastStatus.Approved:
        return 'approved';
      case InterimReportStatus.Rejected:
      case ClaimStatus.Rejected:
      case ProgressReportStatus.Rejected:
      case ForecastStatus.Rejected:
        return 'rejected';
      default:
        return '';
    }
  }

  editInterimReport() {}

  deleteInterimReport() {}

  editClaim() {}

  deleteClaim() {}

  editProgressReport() {}

  deleteProgressReport() {}

  createForecast() {}

  editForecast() {}

  deleteForecast() {}
}
