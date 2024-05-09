import { Component, inject } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { VersionService } from '../../api/version/version.service';
import { VersionInformation } from '../../model';

@Component({
  selector: 'drr-footer',
  standalone: true,
  imports: [MatToolbarModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  versionService = inject(VersionService);

  email = 'EMBCDisasterMitigation@gov.bc.ca';
  versions?: VersionInformation[];

  get apiVersion() {
    return this.versions?.find((v) => v.name === 'EMCR.DRR.API')?.version;
  }

  ngOnInit() {
    this.versionService.versionGetVersionInformation().subscribe((version) => {
      this.versions = version;
    });
  }

  openVersionsModal() {}
}
