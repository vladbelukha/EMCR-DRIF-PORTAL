import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Attachment, FundingStream, ProgramType } from '../../../model';
import { Claim, Forecast, Project, Report } from '../../../model/project';
import { DrrInputComponent } from '../../shared/controls/drr-input/drr-input.component';

@Component({
  selector: 'drr-drif-project',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule,
    DrrInputComponent,
  ],
  templateUrl: './drif-project.component.html',
  styleUrl: './drif-project.component.scss',
})
export class DrifProjectComponent {
  route = inject(ActivatedRoute);

  id?: string;

  project?: Project;

  claimsDataSource = new MatTableDataSource<Claim>([]);
  reportsDataSource = new MatTableDataSource<Report>([]);
  forecastsDataSource = new MatTableDataSource<Forecast>([]);
  attachmentsDataSource = new MatTableDataSource<Attachment>([]);

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];

    // TODO: get project by id

    // TODO: set mock project
    this.project = {
      id: '1',
      projectTitle: 'Water Treatment Plant',
      proponentName: 'City of Richmond',
      fundingStream: FundingStream.Stream1,
      programType: ProgramType.DRIF,
      projectNumber: 'PRJ-123',
      projectStatus: 'Active',
      contacts: [
        {
          firstName: 'John',
          lastName: 'Doe',
          title: 'Manager',
          department: 'Public Works',
          email: 'jd@mail.com',
          phone: '123-456-7890',
        },
      ],
      claims: [],
      reports: [],
      forecast: [],
      attachments: [],
    };

    this.claimsDataSource.data = this.project.claims;
    this.reportsDataSource.data = this.project.reports;
    this.forecastsDataSource.data = this.project.forecast;
    this.attachmentsDataSource.data = this.project.attachments;
  }
}
