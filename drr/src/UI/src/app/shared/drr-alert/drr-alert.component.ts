import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

export type DrrAlertType = 'success' | 'info' | 'warning' | 'danger';

@Component({
  selector: 'drr-alert',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './drr-alert.component.html',
  styleUrl: './drr-alert.component.scss',
})
export class DrrAlertComponent {
  @Input()
  title = '';

  @Input()
  message = '';

  @Input()
  type: DrrAlertType = 'info';
}
