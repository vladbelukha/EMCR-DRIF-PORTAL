import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'drr-drif-progress-report-view',
  standalone: true,
  imports: [],
  templateUrl: './drif-progress-report-view.component.html',
  styleUrl: './drif-progress-report-view.component.scss',
})
export class DrifProgressReportViewComponent {
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
