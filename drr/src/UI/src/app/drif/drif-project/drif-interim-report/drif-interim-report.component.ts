import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'drr-drif-interim-report',
  standalone: true,
  imports: [],
  templateUrl: './drif-interim-report.component.html',
  styleUrl: './drif-interim-report.component.scss',
})
export class DrifInterimReportComponent {
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
