import { Routes } from '@angular/router';
import { AuthenticationGuard } from './core/guards/authentication.guard';
import { DrifEoiViewComponent } from './drif/drif-eoi/drif-eoi-view/drif-eoi-view.component';
import { EOIApplicationComponent } from './drif/drif-eoi/drif-eoi.component';
import { DrifFpInstructionsComponent } from './drif/drif-fp/drif-fp-instructions/drif-fp-instructions.component';
import { DrifFpScreenerComponent } from './drif/drif-fp/drif-fp-screener/drif-fp-screener.component';
import { DrifFpViewComponent } from './drif/drif-fp/drif-fp-view/drif-fp-view.component';
import { DrifFpComponent } from './drif/drif-fp/drif-fp.component';
import { DrifClaimComponent } from './drif/drif-project/drif-claim/drif-claim.component';
import { DrifForecastComponent } from './drif/drif-project/drif-forecast/drif-forecast.component';
import { DrifProgressReportComponent } from './drif/drif-project/drif-progress-report/drif-progress-report.component';
import { DrifProjectComponent } from './drif/drif-project/drif-project.component';
import { SubmissionListComponent } from './submission-list/submission-list.component';

export const routes: Routes = [
  { path: '', component: SubmissionListComponent },
  {
    path: 'submissions',
    component: SubmissionListComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-eoi',
    component: EOIApplicationComponent,
    canActivate: [AuthenticationGuard],
    children: [
      {
        path: ':id',
        component: EOIApplicationComponent,
        canActivate: [AuthenticationGuard],
      },
    ],
  },
  {
    path: 'drif-fp-screener/:eoiId/:projectTitle',
    component: DrifFpScreenerComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-fp-instructions/:eoiId/:projectTitle',
    component: DrifFpInstructionsComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-fp/:id',
    component: DrifFpComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'eoi-submission-details/:id',
    component: DrifEoiViewComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'fp-submission-details/:id',
    component: DrifFpViewComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId',
    component: DrifProjectComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId/progress-report/create',
    component: DrifProgressReportComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId/progress-report/:reportId',
    component: DrifProgressReportComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId/claim/create',
    component: DrifClaimComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId/claim/:claimId',
    component: DrifClaimComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId/forecast/create',
    component: DrifForecastComponent,
    canActivate: [AuthenticationGuard],
  },
  {
    path: 'drif-prj/:projectId/forecast/:forecastId',
    component: DrifForecastComponent,
    canActivate: [AuthenticationGuard],
  },
];
