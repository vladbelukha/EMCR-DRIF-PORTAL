import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { ActivatedRoute } from '@angular/router';
import {
  ClaimStatus,
  ForecastStatus,
  InterimReport,
  InterimReportStatus,
  ProgressReportStatus,
} from '../../../../model/project';

@Component({
  selector: 'drr-drif-interim-report',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatChipsModule, MatButtonModule],
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
      status: InterimReportStatus.Pending,
      description: 'Description 1',
      claim: {
        id: 'CL-0001',
        claimType: 'Claim 1',
        claimDate: '2021-01-01',
        claimAmount: 1000,
        status: ClaimStatus.Review,
      },
      report: {
        id: 'PR-0001',
        reportType: 'Report 1',
        reportDate: '2021-01-01',
        status: ProgressReportStatus.Approved,
      },

      forecast: {
        id: 'FC-0001',
        forecastType: 'Forecast 1',
        forecastDate: '2021-01-01',
        forecastAmount: 1000,
        status: ForecastStatus.Rejected,
      },
    };
  }

  getStatusColorClass(status?: string) {
    switch (status) {
      case InterimReportStatus.Pending:
      case ClaimStatus.Pending:
      case ProgressReportStatus.Pending:
      case ForecastStatus.Pending:
        return 'pending';
      case InterimReportStatus.Review:
      case ClaimStatus.Review:
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

  editForecast() {}

  deleteForecast() {}
}
