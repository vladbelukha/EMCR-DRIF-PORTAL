import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { EntitiesQueryResult } from '../../model';

export type OptionsState = EntitiesQueryResult;

type OptionsStore = {
  entities: OptionsState;
};

const initialState: OptionsState = {
  affectedParties: [],
  capacityRisks: [],
  coBenefits: [],
  complexityRisks: [],
  costConsiderations: [],
  costReductions: [],
  fiscalYears: [],
  increasedResiliency: [],
  professionals: [],
  readinessRisks: [],
  sensitivityRisks: [],
  standards: [],  
  foundationalOrPreviousWorks: [],
  climateAssessmentToolOptions: [],
};

export const OptionsStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setOptions(entities: OptionsState) {
      patchState(store, entities);
    },
    getOptions() {
      return store;
    },
  }))
);
