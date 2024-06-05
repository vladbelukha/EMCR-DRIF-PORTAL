import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface ProfileState {
  name: string;
  email: string;
  organization: string;
}

type ProfileStore = {
  profile: ProfileState;
};

const initialState: ProfileState = {
  name: '',
  email: '',
  organization: '',
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
    setProfile(profile: ProfileState) {
      patchState(store, profile);
    },
  }))
);
