import { Injectable, inject } from '@angular/core';
import { ConfigurationService } from '../../../api/configuration/configuration.service';
import { ConfigurationStore } from '../../store/configuration.store';
import { OptionsStore } from '../../store/options.store';

@Injectable({
  providedIn: 'root',
})
export class AppConfigurationService {
  configurationService = inject(ConfigurationService);
  configurationStore = inject(ConfigurationStore);
  optionsStore = inject(OptionsStore);

  async loadConfiguration() {
    return new Promise((resolve) =>
      this.configurationService.configurationGetConfiguration().subscribe(
        (config) => {
          this.configurationStore.setOidc(config.oidc);
          resolve(true);
        },
        (error) => {
          console.error('Error fetching appConfig', error);
          resolve(false);
        }
      )
    );
  }

  async loadOptions() {
    return this.configurationService
      .configurationGetEntities()
      .subscribe((entities) => {
        this.optionsStore.setOptions({
          ...entities,
        });
      });
  }
}
