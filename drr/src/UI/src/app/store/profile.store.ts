import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface ProfileState {
  fullName: string;
  firstName?: string;
  lastName?: string;
  title?: string;
  department?: string;
  phone?: string;
  email: string;
  organization: string;
  loggedIn: boolean;
}

type ProfileStore = {
  profile: ProfileState;
};

const initialState: ProfileState = {
  fullName: '',
  firstName: '',
  lastName: '',
  title: '',
  department: '',
  phone: '',
  email: '',
  organization: '',
  loggedIn: false,
};

export const ProfileStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setProfile(profile: ProfileState) {
      patchState(store, profile);
    },
    getProfile() {
      return store;
    },
  }))
);
