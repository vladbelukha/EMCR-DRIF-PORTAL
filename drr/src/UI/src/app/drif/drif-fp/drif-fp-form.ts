import { prop, propObject, required } from '@rxweb/reactive-form-validators';
import { FundingStream, ProjectType } from '../../../model';
import { ProponentInformationForm } from '../drif-eoi/drif-eoi-form';

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
  regionalProject?: boolean;

  @prop()
  regionalProjectComments?: string;

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

export class BudgetForm {
  @prop()
  @required()
  totalProjectCost?: number;
}

export class DrifFpForm {
  @propObject(ProponentInformationForm)
  proponentInformation?: ProponentInformationForm =
    new ProponentInformationForm({});

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
