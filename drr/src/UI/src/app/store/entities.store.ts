import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface EntitiesState {
  affectedParties: string[];
  professionals: string[];
}

type EntitiesStore = {
  entities: EntitiesState;
};

const initialState: EntitiesState = {
  affectedParties: [
    'Agricultural Sector',
    'Critical Infrastructure Owners',
    'Crown Corporations',
    'Equity Organization',
    'For Profit And Non-Profit Organizations',
    'Friendship Centres',
    'Neighbouring Jurisdictions',
    'Regional District',
  ],
  professionals: [
    'Accountant',
    'Agrologist',
    'Architect',
    'Biologist',
    'Ecologist',
    'Engineer',
    'Forester',
    'Geologist',
    'Geomorphologist',
    'Geoscientist',
    'Hydrologist',
    'Lawyer',
  ],
};

export const EntitiesStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setEntities(entities: EntitiesState) {
      patchState(store, entities);
    },
    getEntities() {
      return store;
    },
  }))
);
