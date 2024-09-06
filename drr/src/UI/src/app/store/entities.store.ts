import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface EntitiesState {
  affectedParties?: string[];
  professionals?: string[];
  costReductions?: string[];
  coBenefits?: string[];
  verificationMethods?: string[];
  increasedResiliency?: string[];
}

type EntitiesStore = {
  entities: EntitiesState;
};

const initialState: EntitiesState = {
  affectedParties: [],
  professionals: [],
  costReductions: [],
  coBenefits: [],
  verificationMethods: [],
  increasedResiliency: [],
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
