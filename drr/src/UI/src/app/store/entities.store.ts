import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';

export interface EntitiesState {
  affectedParties: string[];
  professionals: string[];
  costReductions: string[];
  coBenefits: string[];
  resiliency: string[];
  verificationMethods: string[];
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
  costReductions: [
    'Emergency response costs',
    'Recovery costs',
    'Disaster-related financial liabilities',
  ],
  coBenefits: [
    'Environmental Co-benefits (e.g. reducing greenhouse gas emissions, enhancing biodiversity, ecosystem services)',
    'Economic Co-benefits (e.g. increase in tourism, support for businesses, increase in property value)',
    'Social Co-benefits (e.g. improving community health and wellbeing, supporting youth voices)',
    'Cultural Co-benefits (e.g. protecting valuable cultural assets)',
    'Multi-hazard Approach (e.g. assessing multiple natural hazards)',
  ],
  resiliency: [
    'Coordination and Engagement',
    'Infrastructure - Existing - Improvements/Upgrades',
    'Infrastructure - Green',
    'Infrastructure - New',
    'Knowledge',
    'Knowledge/Community Education',
    'Planning and Preparedness',
    'Temporary Mitigation Solution',
    'Other',
  ],
  verificationMethods: [
    'Climate change',
    'Event - natural hazard',
    'Foundational project completed - infrastructure assessment',
    'Foundational project completed - mapping',
    'Foundational project completed - other',
    'Foundational project completed - planning',
    'Foundational project completed - risk assessment',
    'Legislation/policy change',
    'Traditional Knowledge',
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
