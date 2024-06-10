import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface ProfileState {
  name: string;
  email: string;
  organization: string;
  loggedIn: boolean;
}

type ProfileStore = {
  profile: ProfileState;
};

const initialState: ProfileState = {
  name: '',
  email: '',
  organization: '',
  loggedIn: false,
};

export const ProfileStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setName(name: string) {
      patchState(store, { name });
    },
    setEmail(email: string) {
      patchState(store, { email });
    },
    setOrganization(organization: string) {
      patchState(store, { organization });
    },
    setLoggedIn(loggedIn: boolean) {
      patchState(store, { loggedIn });
    },
    setProfile(profile: ProfileState) {
      patchState(store, profile);
    },
  }))
);
