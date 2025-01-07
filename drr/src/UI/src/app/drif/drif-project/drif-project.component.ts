import {
  animate,
  state,
  style,
  transition,
  trigger,
} from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import {
  Attachment,
  ContactDetails,
  FundingStream,
  ProgramType,
} from '../../../model';
import {
  Claim,
  ClaimStatus,
  Forecast,
  ForecastStatus,
  InterimReport,
  InterimReportStatus,
  InterimReportType,
  PaymentCondition,
  PaymentConditionStatus,
  ProgressReport,
  ProgressReportStatus,
  Project,
  ProjectStatus,
  ReportingScheduleType,
} from '../../../model/project';
import { DrrInputComponent } from '../../shared/controls/drr-input/drr-input.component';

@Component({
  selector: 'drr-drif-project',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatChipsModule,
    MatTabsModule,
    MatDividerModule,
    DrrInputComponent,
    TranslocoModule,
  ],
  templateUrl: './drif-project.component.html',
  styleUrl: './drif-project.component.scss',
  animations: [
    trigger('detailExpand', [
      state('collapsed,void', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition(
        'expanded <=> collapsed',
        animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')
      ),
    ]),
  ],
})
export class DrifProjectComponent {
  route = inject(ActivatedRoute);
  router = inject(Router);

  projectId?: string;

  project?: Project;

  conditionsDataSource = new MatTableDataSource<PaymentCondition>([]);

  expandedInterimReport?: InterimReport | null;

  projectContactsDataSource = new MatTableDataSource<ContactDetails>([]);

  interimReportsDataSource = new MatTableDataSource<InterimReport>([]);
  pastReportsDataSource = new MatTableDataSource<InterimReport>([]);

  claimsDataSource = new MatTableDataSource<Claim>([]);
  progressReportsDataSource = new MatTableDataSource<ProgressReport>([]);
  forecastsDataSource = new MatTableDataSource<Forecast>([]);
  attachmentsDataSource = new MatTableDataSource<Attachment>([]);

  ngOnInit() {
    this.projectId = this.route.snapshot.params['projectId'];

    // TODO: get project by id

    // TODO: set mock project
    this.project = {
      id: '0001',
      projectTitle: 'Water Treatment Plant',
      proponentName: 'City of Richmond',
      fundingStream: FundingStream.Stream1,
      programType: ProgramType.DRIF,
      projectNumber: 'PRJ-123',
      status: ProjectStatus.Active,
      contractNumber: 'CON-123',
      reportingScheduleType: ReportingScheduleType.Quarterly,
      conditions: [
        {
          id: 'PC-0001',
          conditionName: 'Permits',
          limit: 25,
          status: PaymentConditionStatus.NotMet,
        },
        {
          id: 'PC-0002',
          conditionName: 'Construction start',
          limit: 50,
          status: PaymentConditionStatus.NotMet,
        },
        {
          id: 'PC-0003',
          conditionName: 'Final Report',
          limit: 90,
          status: PaymentConditionStatus.NotMet,
        },
      ],
      startDate: '2021-01-01',
      endDate: '2022-01-01',
      fundingAmount: 1000000,
      eoiId: 'EOI-123',
      fpId: 'FP-123',
      contacts: [
        {
          firstName: 'John',
          lastName: 'Doe',
          title: 'Manager',
          department: 'Public Works',
          email: 'john@mail.com',
          phone: '1111111111',
        },
        {
          firstName: 'Jane',
          lastName: 'Doe',
          title: 'Manager',
          department: 'Public Works',
          email: 'jane@mail.com',
          phone: '2222222222',
        },
      ],
      claims: [
        {
          id: 'CL-0001',
          claimType: 'Claim 1',
          claimDate: '2021-01-01',
          claimAmount: 1000,
          status: ClaimStatus.Pending,
        },
        {
          id: 'CL-0002',
          claimType: 'Claim 2',
          claimDate: '2021-02-01',
          claimAmount: 2000,
          status: ClaimStatus.Review,
        },
      ],
      interimReports: [
        {
          id: 'IR-0001',
          dueDate: '2021-01-01',
          type: InterimReportType.Interim,
          status: InterimReportStatus.Pending,
          claim: {
            id: 'CL-0001',
            claimType: 'Claim 1',
            claimDate: '2021-01-01',
            claimAmount: 1000,
            status: ClaimStatus.Pending,
          },
          report: {
            id: 'PR-0001',
            reportType: 'Report 1',
            reportDate: '2021-01-01',
            status: ProgressReportStatus.Pending,
          },
          forecast: {
            id: 'FC-0001',
            forecastType: 'Forecast 1',
            forecastDate: '2021-01-01',
            forecastAmount: 1000,
            status: ForecastStatus.Pending,
          },
        },
        {
          id: 'IR-0002',
          dueDate: '2021-02-01',
          type: InterimReportType.Interim,
          status: InterimReportStatus.Review,
          claim: {
            id: 'CL-0002',
            claimType: 'Claim 2',
            claimDate: '2021-02-01',
            claimAmount: 2000,
            status: ClaimStatus.Review,
          },
          report: {
            id: 'IR-0002',
            reportType: 'Report 2',
            reportDate: '2021-02-01',
            status: ProgressReportStatus.Review,
          },

          forecast: {
            id: 'FC-0002',
            forecastType: 'Forecast 2',
            forecastDate: '2021-02-01',
            forecastAmount: 2000,
            status: ForecastStatus.Review,
          },
        },
        {
          id: 'IR-0003',
          dueDate: '2021-03-01',
          type: InterimReportType.Interim,
          status: InterimReportStatus.Approved,
          claim: {
            id: 'CL-0003',
            claimType: 'Claim 3',
            claimDate: '2021-03-01',
            claimAmount: 3000,
            status: ClaimStatus.Approved,
          },
          report: {
            id: 'IR-0003',
            reportType: 'Report 3',
            reportDate: '2021-03-01',
            status: ProgressReportStatus.Approved,
          },
          forecast: {
            id: 'FC-0003',
            forecastType: 'Forecast 3',
            forecastDate: '2021-03-01',
            forecastAmount: 3000,
            status: ForecastStatus.Approved,
          },
        },
        {
          id: 'IR-0004',
          dueDate: '2021-04-01',
          type: InterimReportType.Interim,
          status: InterimReportStatus.Rejected,
          claim: {
            id: 'CL-0004',
            claimType: 'Claim 4',
            claimDate: '2021-04-01',
            claimAmount: 4000,
            status: ClaimStatus.Rejected,
          },
          report: {
            id: 'IR-0004',
            reportType: 'Report 4',
            reportDate: '2021-04-01',
            status: ProgressReportStatus.Rejected,
          },
          forecast: {
            id: 'FC-0004',
            forecastType: 'Forecast 4',
            forecastDate: '2021-04-01',
            forecastAmount: 4000,
            status: ForecastStatus.Rejected,
          },
        },
        {
          id: 'IR-0005',
          dueDate: '2021-05-01',
          type: InterimReportType.Interim,
          status: InterimReportStatus.Rejected,
          claim: {
            id: 'CL-0005',
            claimType: 'Claim 5',
            claimDate: '2021-05-01',
            claimAmount: 5000,
            status: ClaimStatus.Pending,
          },
          report: {
            id: 'IR-0005',
            reportType: 'Report 5',
            reportDate: '2021-05-01',
            status: ProgressReportStatus.Pending,
          },
          forecast: {
            id: 'FC-0005',
            forecastType: 'Forecast 5',
            forecastDate: '2021-05-01',
            forecastAmount: 5000,
            status: ForecastStatus.Pending,
          },
        },
      ],
      progressReports: [
        {
          id: 'PR-0001',
          reportType: 'Report 1',
          reportDate: '2021-01-01',
          status: ProgressReportStatus.Pending,
        },
        {
          id: 'PR-0002',
          reportType: 'Report 2',
          reportDate: '2021-02-01',
          status: ProgressReportStatus.Review,
        },
      ],
      forecast: [
        {
          id: 'FC-0001',
          forecastType: 'Forecast 1',
          forecastDate: '2021-01-01',
          forecastAmount: 1000,
          status: ForecastStatus.Pending,
        },
        {
          id: 'FC-0002',
          forecastType: 'Forecast 2',
          forecastDate: '2021-02-01',
          forecastAmount: 2000,
          status: ForecastStatus.Review,
        },
      ],
      attachments: [],
    };

    this.conditionsDataSource.data = this.project.conditions;

    this.projectContactsDataSource.data = this.project.contacts;

    this.interimReportsDataSource.data = this.project.interimReports.filter(
      (report) =>
        report.status !== InterimReportStatus.Approved &&
        report.status !== InterimReportStatus.Rejected
    );
    this.pastReportsDataSource.data = this.project.interimReports.filter(
      (report) =>
        report.status === InterimReportStatus.Approved ||
        report.status === InterimReportStatus.Rejected
    );

    this.claimsDataSource.data = this.project.claims;
    this.progressReportsDataSource.data = this.project.progressReports;
    this.forecastsDataSource.data = this.project.forecast;
    this.attachmentsDataSource.data = this.project.attachments;
  }

  addInterimReport() {
    // TODO: create report prior to navigating?

    this.router.navigate([
      'drif-projects',
      this.projectId,
      'interim-reports',
      'create',
    ]);
  }

  addProjectContact() {}

  viewClaimClick(claim: Claim, event: Event) {
    event.stopPropagation();

    this.router.navigate(['drif-projects', this.projectId, 'claims', claim.id]);
  }

  editClaim() {}

  deleteClaim() {}

  editProgressReport() {}

  deleteProgressReport() {}

  editForecast() {}

  deleteForecast() {}
}
