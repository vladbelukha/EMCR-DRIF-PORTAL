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
    TranslocoModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  router = inject(Router);
  profileStore = inject(ProfileStore);
  activatedRoute = inject(ActivatedRoute);

  notifications = [
    {
      title: 'New application was created for your organization',
      message: 'John Doe created a new application for your organization',
    },
    {
      title: 'Your application got rejected',
      message: 'Your application was rejected by the DMAP team',
    },
    {
      title: 'Your application status has been updated',
      message: 'Your application status has been updated to "Under Review"',
    },
  ];

  applications = [
    {
      id: '1',
      title: 'Park renovation project application',
      status: 'Draft',
      date: '2021-06-01',
    },
    {
      id: '2',
      title: 'New construction project application',
      status: 'Submitted',
      date: '2021-04-05',
    },
    {
      id: '3',
      title: 'Environmental assessment application',
      status: 'Under Review',
      date: '2021-05-16',
    },
    {
      id: '4',
      title: 'Beach cleanup project application',
      status: 'Rejected',
      date: '2021-03-01',
    },
  ];

  ngOnInit() {
    if (this.activatedRoute.snapshot.queryParams['state']) {
      this.router.navigate([], { queryParams: {}, skipLocationChange: true });
    }
  }

  onCreateFormClick() {
    this.router.navigate(['/eoi-application']);
  }

  onViewFormClick(id: string) {
    this.router.navigate(['/eoi-application']);
  }
}
