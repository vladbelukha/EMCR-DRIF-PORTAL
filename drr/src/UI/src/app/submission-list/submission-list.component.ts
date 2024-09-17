import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router, RouterModule } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { prop, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { ConditionalOperator, GridifyQueryBuilder } from 'gridify-client';
import { distinctUntilChanged } from 'rxjs';
import { DrifapplicationService } from '../../api/drifapplication/drifapplication.service';
import {
  ApplicationType,
  ProgramType,
  Submission,
  SubmissionPortalStatus,
} from '../../model';
import { DrrSelectComponent } from '../shared/controls/drr-select/drr-select.component';

class SubmissionFilter {
  @prop()
  programType?: ProgramType;

  @prop()
  applicationType?: ApplicationType;

  @prop()
  status?: SubmissionPortalStatus[];
}

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drr-submission-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    TranslocoModule,
    RouterModule,
    DrrSelectComponent,
  ],
  templateUrl: './submission-list.component.html',
  styleUrl: './submission-list.component.scss',
  providers: [RxFormBuilder, GridifyQueryBuilder],
})
export class SubmissionListComponent {
  router = inject(Router);
  applicationService = inject(DrifapplicationService);
  formbuilder = inject(RxFormBuilder);

  programTypeOptions = Object.values(ProgramType).map((value) => ({
    value,
    label: value,
  }));
  applicationTypeOptions = Object.values(ApplicationType).map((value) => ({
    value,
    label: value,
  }));
  statusOptions = Object.values(SubmissionPortalStatus).map((value) => ({
    value,
    label: value,
  }));

  showFilters = false;
  filterApplied = true;
  filterForm = this.formbuilder.formGroup(SubmissionFilter);

  submissions?: Submission[];

  submissionListColumns = [
    'id',
    'programType',
    'applicationType',
    'projectTitle',
    'status',
    'fundingRequest',
    'modifiedDate',
    'submittedDate',
    'partneringProponents',
    'actions',
  ];
  submissionListDataSource = new MatTableDataSource<Submission>();

  paginator = {
    length: 0,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 100],
    pageIndex: 0,
    showPaginator: false,
  };
  sort: Sort = {
    active: 'id',
    direction: 'desc',
  };

  ngOnInit() {
    this.load();

    this.filterForm.valueChanges.pipe(distinctUntilChanged()).subscribe(() => {
      this.filterApplied = false;
    });
  }

  load() {
    const query = this.getQuery();

    this.applicationService
      .dRIFApplicationGet({
        Filter: query.filter,
        OrderBy: query.orderBy,
        Page: query.page,
        PageSize: query.pageSize,
      })
      .subscribe((submissions) => {
        this.submissions = submissions;
        this.submissionListDataSource = new MatTableDataSource(
          this.submissions
        );
        this.paginator.length = submissions.length;
        this.paginator.showPaginator =
          submissions.length > this.paginator.pageSize;
      });
  }

  onPageChange(event: PageEvent) {
    this.paginator.pageIndex = event.pageIndex;
    this.paginator.pageSize = event.pageSize;

    this.load();
  }

  onSortSubmissionTable(sort: Sort) {
    this.sort = sort;

    this.paginator.pageIndex = 0;

    this.load();
  }

  onCreateFormClick() {
    this.router.navigate(['/drif-eoi']);
  }

  onViewFormClick(submission: Submission, event: Event) {
    event.preventDefault();

    submission.applicationType === 'EOI'
      ? this.router.navigate([
          submission.status == 'Draft'
            ? '/drif-eoi'
            : '/eoi-submission-details',
          submission.id,
        ])
      : this.router.navigate([
          submission.status == 'Draft' ? '/drif-fp' : '/fp-submission-details',
          submission.id,
        ]);
  }

  canCreateFullProposal(submission: Submission) {
    return submission.status === 'EligibleInvited' && !submission.existingFpId;
  }

  createFullProposal(submission: Submission, event: Event) {
    event.preventDefault();

    this.router.navigate([
      '/drif-fp-screener',
      submission.id,
      submission.fundingStream,
    ]);
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  applyFilters() {
    this.filterApplied = true;
    this.paginator.pageIndex = 0;
    this.load();
  }

  getQuery() {
    const filters = this.filterForm.value as SubmissionFilter;

    const query = new GridifyQueryBuilder()
      .setPage(this.paginator.pageIndex)
      .setPageSize(this.paginator.pageSize)
      .addOrderBy(this.sort.active, this.sort.direction === 'desc')
      .addCondition(
        'programType',
        ConditionalOperator.Equal,
        filters.programType ?? ''
      )
      .and()
      .addCondition(
        'applicationType',
        ConditionalOperator.Equal,
        filters.applicationType ?? ''
      )
      .and()
      .addCondition(
        'status',
        ConditionalOperator.Contains,
        filters.status ? filters.status.join('|') : ''
      )
      .build();

    return query;
  }

  clearFilters() {
    if (this.filterForm.untouched) {
      this.toggleFilters();
      return;
    }

    this.paginator.pageIndex = 0;
    this.filterForm.reset({ emitEvent: false });
    this.filterApplied = true;
    this.load();
    this.toggleFilters();
  }
}
