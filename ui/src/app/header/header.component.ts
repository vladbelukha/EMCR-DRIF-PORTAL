import { CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'drr-header',
  standalone: true,
  imports: [CommonModule, MatDividerModule,  MatToolbarModule, MatMenuModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class HeaderComponent {
  homeClick() {}

  signOut() {}

  isAuthenticated() {
    return false;
  }
}
