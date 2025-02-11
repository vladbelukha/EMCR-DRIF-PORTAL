import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import {
  ApplicationType,
  DeclarationInfo,
  DeclarationType,
  EntitiesQueryResult,
  FormType,
} from '../../model';

export type OptionsState = EntitiesQueryResult;
export type DeclarationState = DeclarationInfo[];

type OptionsStore = {
  options: OptionsState;
  declarations: DeclarationState;
};

const initialState: OptionsStore = {
  options: {
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
    projectActivities: [],
  },
  declarations: [],
};

export const OptionsStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    setOptions(options: OptionsState) {
      patchState(store, { options });
    },
    getOptions() {
      return store.options;
    },
    setDeclarations(declarations: DeclarationState) {
      patchState(store, { declarations });
    },
    getDeclarations(
      declarationType: DeclarationType,
      applicationType: ApplicationType,
      formType: FormType,
    ): string | undefined {
      return store
        .declarations()
        .find(
          (d) =>
            d.type === declarationType &&
            d.applicationType === applicationType &&
            d.formType === formType,
        )?.text;
    },
  })),
);
