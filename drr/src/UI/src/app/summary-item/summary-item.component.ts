import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { RxFormControl } from '@rxweb/reactive-form-validators';

@Component({
  selector: 'drr-summary-item',
  standalone: true,
  imports: [CommonModule, MatInputModule],
  templateUrl: './summary-item.component.html',
  styleUrl: './summary-item.component.scss',
})
export class SummaryItemComponent {
  @Input() label?: string;
  @Input() value?: string | null | undefined;
  @Input() rxFormControl?: RxFormControl;
}
