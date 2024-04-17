import { Component, Input } from '@angular/core';
import { EOIApplication } from '../../model';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'drr-summary',
  standalone: true,
  imports: [CommonModule, MatInputModule],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
  @Input()
  eoiApplication?: EOIApplication;

  objectHasValues(obj: any) {
    return obj && Object.values(obj).some((value) => !!value);
  }

  arrayHasValues(array: any[]) {
    return (
      array &&
      array.length > 0 &&
      array.some((value) => this.objectHasValues(value))
    );
  }
}
