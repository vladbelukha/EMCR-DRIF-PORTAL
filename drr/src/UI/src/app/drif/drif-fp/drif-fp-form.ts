import {
  minLength,
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import {
  FundingStream,
  Hazards,
  ProjectType,
  ProponentType,
} from '../../../model';
import {
  ContactDetailsForm,
  FundingInformationItemForm,
  ProponentInformationForm,
  StringItem,
} from '../drif-eoi/drif-eoi-form';

// TODO: temp before API provides the correct structure
export class FileForm {
  @prop()
  name?: string;

  @prop()
  url?: string;

  @prop()
  type?: string;

  @prop()
  id?: string;

  @prop()
  comment?: string;
}

export class OwnershipAndAuthorizationForm {
  @prop()
  @required()
  ownership?: boolean;

  @prop()
  ownershipComments?: string;

  @prop()
  @required()
  authorityAndOwnership?: boolean;

  @prop()
  @required()
  authorityAndOwnershipComments?: string;

  @prop()
  @required()
  operationAndMaintenance?: boolean;

  @prop()
  @required()
  operationAndMaintenanceComments?: string;

  @prop()
  @required()
  firstNationsEndorsement?: number; // TODO: change to enum

  @prop()
  @required()
  localGovernmentEndorsement?: number; // TODO: change to enum

  @prop()
  @required()
  authorizationOrEndorsementComments?: string;

  constructor(values: OwnershipAndAuthorizationForm) {
    Object.assign(this, values);
  }
}

export class YearOverYearFundingForm {
  @prop()
  @required()
  year?: number;

  @prop()
  @required()
  amount?: number;
}

export class BudgetForm {
  @prop()
  @required()
  totalProjectCost?: number;

  @prop()
  @required()
  fundingRequest?: number;

  @propArray(YearOverYearFundingForm)
  yearOverYearFunding?: YearOverYearFundingForm[] = [{}];

  @prop()
  @required()
  totalDrifFundingRequest?: number;

  @prop()
  discrepancyComment?: string;

  @propArray(FundingInformationItemForm)
  otherFunding?: FundingInformationItemForm[] = [{}];
}

export class ProponentAndProjectInformationForm {
  @prop()
  @required()
  projectTitle?: string;

  @prop()
  @required()
  scopeStatement?: string;

  @prop()
  @required()
  proponentType?: ProponentType;

  @prop()
  @required()
  proponentName?: string;

  @required()
  @propObject(ContactDetailsForm)
  projectContact?: ContactDetailsForm = new ContactDetailsForm({});

  @required()
  @propArray(ContactDetailsForm)
  additionalContacts?: ContactDetailsForm[] = [{}];

  @prop()
  partneringProponents?: string[] = [];

  @propArray(StringItem)
  partneringProponentsArray?: StringItem[] = [{ value: '' }];

  @prop()
  @required()
  regionalProject?: boolean;

  @prop()
  regionalProjectComments?: string;

  @prop()
  @required()
  @minLength({ value: 1 })
  relatedHazards?: Hazards[];

  @prop()
  otherHazardsDescription?: string;

  constructor(values: ProponentInformationForm) {
    Object.assign(this, values);
  }
}

export class DrifFpForm {
  @propObject(ProponentAndProjectInformationForm)
  proponentAndProjectInformationForm?: ProponentAndProjectInformationForm =
    new ProponentAndProjectInformationForm({});

  @propObject(OwnershipAndAuthorizationForm)
  ownershipAndAuthorization?: OwnershipAndAuthorizationForm =
    new OwnershipAndAuthorizationForm({});

  @propObject(BudgetForm)
  budget?: BudgetForm = new BudgetForm();

  @prop()
  eoiId?: string;

  @prop()
  fundingStream?: FundingStream;

  @prop()
  projectType?: ProjectType;
  // TODO: have a factory method to create a new instance of this form
}
