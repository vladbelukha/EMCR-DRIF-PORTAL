import {
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import { FundingStream, ProjectType, ProponentType } from '../../../model';
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

export class ProponentEligibilityForm {
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

  // TODO: supportingDocuments should be an array of file type capturing name, url, type, id and comment?

  constructor(values: ProponentEligibilityForm) {
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

  constructor(values: ProponentInformationForm) {
    Object.assign(this, values);
  }
}

export class DrifFpForm {
  @propObject(ProponentAndProjectInformationForm)
  proponentAndProjectInformationForm?: ProponentAndProjectInformationForm =
    new ProponentAndProjectInformationForm({});

  @propObject(ProponentEligibilityForm)
  proponentEligibility?: ProponentEligibilityForm =
    new ProponentEligibilityForm({});

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
