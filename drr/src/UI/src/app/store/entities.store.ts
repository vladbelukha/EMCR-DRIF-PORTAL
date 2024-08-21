import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface EntitiesState {
  affectedParties: string[];
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
