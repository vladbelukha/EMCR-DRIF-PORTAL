import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface ConfigurationState {
  oidc?: {
    clientId?: string;
    issuer?: string;
    scope?: string;
    postLogoutRedirectUrl?: string;
    accountRecoveryUrl?: string;
  };
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
};

export const ConfigurationStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setOidc(oidc: ConfigurationState['oidc']) {
      patchState(store, { oidc });
    },
  }))
);
