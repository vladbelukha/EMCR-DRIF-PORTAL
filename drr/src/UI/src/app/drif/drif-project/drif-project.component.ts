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
import { ProjectService } from '../../../api/project/project.service';
import {
  Attachment,
  ContactDetails,
  DraftDrrProject,
  InterimReport,
  InterimReportStatus,
  PaymentCondition,
  ProjectClaim,
} from '../../../model';
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
  projectService = inject(ProjectService);

  projectId?: string;

  project?: DraftDrrProject;

  conditionsDataSource = new MatTableDataSource<
    PaymentCondition & { actions?: [] }
  >([]);

  expandedInterimReport?: InterimReport | null;

  projectContactsDataSource = new MatTableDataSource<ContactDetails>([]);

  interimReportsDataSource = new MatTableDataSource<InterimReport>([]);
  pastReportsDataSource = new MatTableDataSource<InterimReport>([]);

  attachmentsDataSource = new MatTableDataSource<Attachment>([]);

  ngOnInit() {
    this.projectId = this.route.snapshot.params['projectId'];

    this.projectService
      .projectGetProject(this.projectId!)
      .subscribe((project) => {
        this.project = project;

        this.conditionsDataSource.data = [
          ...this.project!.conditions!,
          {
            id: 'PC-0004',
            conditionName: 'Condition 4',
            limit: 90,
            conditionMet: false,
          },
          {
            id: 'PC-0005',
            conditionName: 'Condition 5',
            limit: 90,
            conditionMet: false,
          },
        ];

        this.projectContactsDataSource.data = this.project!.contacts!;

        this.interimReportsDataSource.data =
          this.project!.interimReports!.filter(
            (report) =>
              report.status !== InterimReportStatus.Approved &&
              report.status !== InterimReportStatus.Skipped
          );
        this.pastReportsDataSource.data = this.project!.interimReports!.filter(
          (report) =>
            report.status === InterimReportStatus.Approved ||
            report.status === InterimReportStatus.Skipped
        );

        this.attachmentsDataSource.data = this.project!.attachments!;
      });
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

  editClaim() {}

  deleteClaim() {}

  editProgressReport() {}

  deleteProgressReport() {}

  editForecast() {}

  deleteForecast() {}

  viewContion() {}

  clearCondition() {}
}
