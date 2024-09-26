import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router, RouterModule } from '@angular/router';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { prop, RxFormBuilder } from '@rxweb/reactive-form-validators';
import {
  ConditionalOperator,
  GridifyQueryBuilder,
  IGridifyQuery,
} from 'gridify-client';
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
  translocoService = inject(TranslocoService);

  programTypeOptions = Object.values(ProgramType).map((value) => ({
    value,
    label: this.translocoService.translate(value),
  }));
  applicationTypeOptions = Object.values(ApplicationType).map((value) => ({
    value,
    label: this.translocoService.translate(value),
  }));
  statusOptions = Object.values(SubmissionPortalStatus)
    .filter((status) => status !== SubmissionPortalStatus.ApprovedInPrinciple)
    .map((value) => ({
      value,
      label: this.translocoService.translate(value),
    }))
    .sort((a, b) => a.label.localeCompare(b.label));

  filterApplied = false;
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
      .subscribe((response) => {
        this.submissions = response.submissions;
        this.submissionListDataSource = new MatTableDataSource(
          this.submissions
        );
        this.paginator.length = response.length!;
      });
  }

  onPageChange(event: PageEvent) {
    this.paginator.pageIndex = event.pageIndex;
    this.paginator.pageSize = event.pageSize;

    this.load();
  }

  onSortSubmissionTable(sort: Sort) {
    if (sort.direction === '') {
      this.sort.active = 'id';
      this.sort.direction = 'desc';
    } else {
      this.sort = sort;
    }

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
      submission.projectTitle,
    ]);
  }

  getQuery(): IGridifyQuery {
    const filters = this.filterForm.value as SubmissionFilter;

    const query = new GridifyQueryBuilder()
      .setPage(this.paginator.pageIndex)
      .setPageSize(this.paginator.pageSize)
      .addOrderBy(this.sort.active, this.sort.direction === 'desc');

    if (filters.programType) {
      query.addCondition(
        'programType',
        ConditionalOperator.Equal,
        filters.programType ?? ''
      );
    }

    if (filters.programType && filters.applicationType) {
      query.and();
    }

    if (filters.applicationType) {
      query.addCondition(
        'applicationType',
        ConditionalOperator.Equal,
        filters.applicationType ?? ''
      );
    }

    if (
      (filters.applicationType || filters.programType) &&
      filters.status &&
      filters.status.length > 0
    ) {
      query.and();
    }

    if (filters.status && filters.status.length > 0) {
      query.addCondition(
        'status',
        ConditionalOperator.Equal,
        filters.status.join('|')
      );
    }

    return query.build();
  }

  applyFilters() {
    if (this.filterForm.pristine) {
      return;
    }

    this.filterApplied = true;
    this.paginator.pageIndex = 0;
    this.load();
  }

  clearFilters() {
    if (this.filterForm.pristine) {
      return;
    }

    this.paginator.pageIndex = 0;
    this.filterForm.reset({ emitEvent: false });
    this.filterForm.markAsPristine();
    this.filterApplied = false;
    this.load();
  }
}
