import { prop, propObject, required } from '@rxweb/reactive-form-validators';
import { FundingStream, ProjectType } from '../../../model';
import { ProponentInformationForm } from '../drif-eoi/drif-eoi-form';

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

  // TODO: supportDocuments: File[];

  constructor(values: ProponentEligibilityForm) {
    Object.assign(this, values);
  }
}

export class DrifFpForm {
  @propObject(ProponentInformationForm)
  proponentInformation?: ProponentInformationForm =
    new ProponentInformationForm({});

  @propObject(ProponentEligibilityForm)
  proponentEligibility?: ProponentEligibilityForm =
    new ProponentEligibilityForm({});

  @prop()
  eoiId?: string;

  @prop()
  fundingStream?: FundingStream;

  @prop()
  projectType?: ProjectType;
  // TODO: have a factory method to create a new instance of this form
}
