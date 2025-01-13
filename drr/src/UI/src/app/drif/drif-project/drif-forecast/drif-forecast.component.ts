import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'drr-drif-forecast',
  standalone: true,
  imports: [],
  templateUrl: './drif-forecast.component.html',
  styleUrl: './drif-forecast.component.scss',
})
export class DrifForecastComponent {
  route = inject(ActivatedRoute);

  projectId?: string;
  forecastId?: string;

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
      this.forecastId = params['forecastId'];
    });
  }
}
