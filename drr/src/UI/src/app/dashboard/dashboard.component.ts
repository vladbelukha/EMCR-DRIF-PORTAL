import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';

import { ProfileStore } from '../store/profile.store';
import { SubmissionListComponent } from './submission-list/submission-list.component';

@Component({
  selector: 'drr-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatListModule,
    SubmissionListComponent,
    TranslocoModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  router = inject(Router);
  profileStore = inject(ProfileStore);
  activatedRoute = inject(ActivatedRoute);

  ngOnInit() {
    if (this.activatedRoute.snapshot.queryParams['state']) {
      this.router.navigate([], { queryParams: {}, skipLocationChange: true });
    }
  }

  onCreateFormClick() {
    this.router.navigate(['/drif-eoi']);
  }
}
