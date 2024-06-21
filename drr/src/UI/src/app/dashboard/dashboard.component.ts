import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { DrifapplicationService } from '../../api/drifapplication/drifapplication.service';
import { CRAFTApplication } from '../../model';
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
    MatChipsModule,
    TranslocoModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  router = inject(Router);
  profileStore = inject(ProfileStore);
  activatedRoute = inject(ActivatedRoute);
  applicationService = inject(DrifapplicationService);

  applications?: CRAFTApplication[];

  ngOnInit() {
    if (this.activatedRoute.snapshot.queryParams['state']) {
      this.router.navigate([], { queryParams: {}, skipLocationChange: true });
    }

    this.applicationService
      .dRIFApplicationGetAll()
      .subscribe((applications) => {
        this.applications = applications;
      });
  }

  onCreateFormClick() {
    this.router.navigate(['/eoi-application']);
  }

  onViewFormClick(id: string) {
    this.router.navigate(['/eoi-application', id]);
  }
}
