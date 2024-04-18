import { Component, Input } from '@angular/core';
import { DrifEoiApplication } from '../../model';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { TranslocoModule } from '@ngneat/transloco';

@Component({
  selector: 'drr-summary',
  standalone: true,
  imports: [CommonModule, MatInputModule, TranslocoModule],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
  @Input()
  eoiApplication?: DrifEoiApplication;

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
