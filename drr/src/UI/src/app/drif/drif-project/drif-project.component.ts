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
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
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
  Forecast,
  InterimReport,
  ProgressReport,
  Project,
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

  expandedInterimReport?: InterimReport | null;

  projectContactsDataSource = new MatTableDataSource<ContactDetails>([]);

  interimReportsDataSource = new MatTableDataSource<InterimReport>([]);
  
  

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
      projectStatus: 'Active',
      contractNumber: 'CON-123',
      reportingScheduleType: ReportingScheduleType.Quarterly,
      startDate: '2021-01-01',
      endDate: '2022-01-01',
      fundingAmount: 1000000,
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
          claimStatus: 'Pending',
        },
        {
          id: 'CL-0002',
          claimType: 'Claim 2',
          claimDate: '2021-02-01',
          claimAmount: 2000,
          claimStatus: 'Review',
        },
      ],
      interimReports: [
        {
          id: 'IR-0001',
          reportDate: '2021-01-01',
          reportStatus: 'Pending',
          claim: {
            id: 'CL-0001',
            claimType: 'Claim 1',
            claimDate: '2021-01-01',
            claimAmount: 1000,
            claimStatus: 'Pending',
          },
          report: {
            id: 'IR-0001',
            reportType: 'Report 1',
            reportDate: '2021-01-01',
            reportStatus: 'Pending',
          },

          forecast: {
            id: 'FC-0001',
            forecastType: 'Forecast 1',
            forecastDate: '2021-01-01',
            forecastAmount: 1000,
            forecastStatus: 'Pending',
          },
        },
        {
          id: 'IR-0002',
          reportDate: '2021-02-01',
          reportStatus: 'Review',
          claim: {
            id: 'CL-0002',
            claimType: 'Claim 2',
            claimDate: '2021-02-01',
            claimAmount: 2000,
            claimStatus: 'Review',
          },
          report: {
            id: 'IR-0002',
            reportType: 'Report 2',
            reportDate: '2021-02-01',
            reportStatus: 'Review',
          },

          forecast: {
            id: 'FC-0002',
            forecastType: 'Forecast 2',
            forecastDate: '2021-02-01',
            forecastAmount: 2000,
            forecastStatus: 'Review',
          },
        },
      ],
      progressReports: [
        {
          id: 'PR-0001',
          reportType: 'Report 1',
          reportDate: '2021-01-01',
          reportStatus: 'Pending',
        },
        {
          id: 'PR-0002',
          reportType: 'Report 2',
          reportDate: '2021-02-01',
          reportStatus: 'Review',
        },
      ],
      forecast: [
        {
          id: 'FC-0001',
          forecastType: 'Forecast 1',
          forecastDate: '2021-01-01',
          forecastAmount: 1000,
          forecastStatus: 'Pending',
        },
        {
          id: 'FC-0002',
          forecastType: 'Forecast 2',
          forecastDate: '2021-02-01',
          forecastAmount: 2000,
          forecastStatus: 'Review',
        },
      ],
      attachments: [],
    };

    this.projectContactsDataSource.data = this.project.contacts;

    this.interimReportsDataSource.data = this.project.interimReports;

    this.claimsDataSource.data = this.project.claims;
    this.progressReportsDataSource.data = this.project.progressReports;
    this.forecastsDataSource.data = this.project.forecast;
    this.attachmentsDataSource.data = this.project.attachments;
  }

  addInterimReport() {}

  addProjectContact() {}

  viewClaimClick(claim: Claim, event: Event) {
    event.stopPropagation();

    this.router.navigate(['drif-prj', this.projectId, 'claims', claim.id]);
  }
}
