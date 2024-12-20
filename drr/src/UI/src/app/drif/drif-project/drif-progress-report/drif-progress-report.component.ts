import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'drr-drif-progress-report',
  standalone: true,
  imports: [],
  templateUrl: './drif-progress-report.component.html',
  styleUrl: './drif-progress-report.component.scss',
})
export class DrifProgressReportComponent {
  route = inject(ActivatedRoute);

  projectId?: string;
  reportId?: string;

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
      this.reportId = params['reportId'];
    });
  }
}
