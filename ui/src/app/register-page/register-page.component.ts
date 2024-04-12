import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'drr-register-page',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss',
})
export class RegisterPageComponent {
  startApplication() {
    // This is where you would start your application.
  }
}
