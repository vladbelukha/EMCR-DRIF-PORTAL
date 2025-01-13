import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslocoModule } from '@ngneat/transloco';
import { ProjectListComponent } from './project-list/project-list.component';
import { SubmissionListComponent } from './submission-list/submission-list.component';

@Component({
  selector: 'drr-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    TranslocoModule,
    ProjectListComponent,
    SubmissionListComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {}
