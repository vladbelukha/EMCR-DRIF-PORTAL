import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { EOIApplicationForm } from '../eoi-application/eoi-application-form';

@Component({
  selector: 'drr-summary',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
  @Input()
  eoiApplication?: EOIApplicationForm;

  objectHasValues(obj: any): boolean {
    // if array - check length, if value - check if truthy
    return (
      obj &&
      Object.values(obj).some((value) => {
        if (Array.isArray(value)) {
          return this.arrayHasValues(value);
        } else {
          return !!value;
        }
      })
    );
  }

  arrayHasValues(array: any[]): boolean {
    return (
      array &&
      array.length > 0 &&
      array.some((value) => this.objectHasValues(value))
    );
  }
}
