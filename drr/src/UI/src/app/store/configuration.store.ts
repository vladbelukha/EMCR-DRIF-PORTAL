import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface ConfigurationState {
  oidc?: {
    clientId?: string;
    issuer?: string;
    scope?: string;
    postLogoutRedirectUrl?: string;
    accountRecoveryUrl?: string;
  };
  isConfigurationLoaded: boolean;
}

type ConfigurationStore = {
  configuration: ConfigurationState;
};

const initialState: ConfigurationState = {
  oidc: {
    clientId: '',
    issuer: '',
    scope: '',
    postLogoutRedirectUrl: '',
    accountRecoveryUrl: '',
  },
  isConfigurationLoaded: false,
};

export const ConfigurationStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setOidc(oidc: ConfigurationState['oidc']) {
      patchState(store, { oidc, isConfigurationLoaded: true });
    },
  }))
);
