import {
  disable,
  email,
  minLength,
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import {
  ProponentType,
  ContactDetails,
  DrifEoiApplication,
  FundingInformation,
  FundingType,
  Hazards,
  ProjectType,
  FundingStream,
} from '../../model';

export class FundingInformationForm implements FundingInformation {
  @prop()
  amount?: number;
  @prop()
  name?: string;
  @prop()
  type?: FundingType;
  @prop()
  description?: string;

  constructor(values: FundingInformationForm) {
    Object.assign(this, values);
  }
}

export class ContactDetailsForm implements ContactDetails {
  @prop()
  @required()
  @email()
  email?: string;

  @prop()
  @required()
  firstName?: string;

  @prop()
  @required()
  lastName?: string;

  @prop()
  @required()
  phone?: string;

  @prop()
  @required()
  department?: string;

  @prop()
  @required()
  title?: string;

  constructor(values: ContactDetailsForm) {
    Object.assign(this, values);
  }
}

export class StringItem {
  @prop()
  value: string = '';

  constructor(values: StringItem) {
    Object.assign(this, values);
  }
}

export class EOIApplicationForm implements DrifEoiApplication {
  @prop()
  @required()
  proponentName?: string;

  @prop()
  @required()
  proponentType?: ProponentType;

  @prop()
  @required()
  climateAdaptation?: string;

  @prop()
  coordinates?: string;

  @prop()
  @required()
  fundingStream?: FundingStream;

  @prop()
  @required()
  endDate?: string;

  @prop()
  @required()
  firstNationsEngagement?: string;

  @prop()
  @required()
  neighbourEngagement?: string;

  @prop()
  @required()
  estimatedTotal?: number;

  @prop()
  @required()
  fundingRequest?: number;

  @propArray(FundingInformationForm)
  otherFunding?: FundingInformationForm[] = [];

  @prop()
  otherInformation?: string;

  @prop()
  @required()
  ownershipDeclaration?: boolean;

  @prop()
  ownershipDescription?: string;

  @prop()
  locationDescription?: string;

  @propArray(ContactDetailsForm)
  additionalContacts?: ContactDetailsForm[] = [{}];

  @prop()
  partneringProponents?: string[] = [];

  @propArray(StringItem)
  partneringProponentsArray?: StringItem[] = [{ value: '' }];

  @prop()
  @required()
  projectTitle?: string;

  @prop()
  @required()
  projectType?: ProjectType;

  @prop()
  @required()
  infrastructureImpacted?: string[] = [];

  @propArray(StringItem)
  infrastructureImpactedArray?: StringItem[] = [{ value: '' }];

  @prop()
  @required()
  disasterRiskUnderstanding?: string;

  @prop()
  additionalBackgroundInformation?: string;

  @prop()
  additionalEngagementInformation?: string;

  @prop()
  @required()
  addressRisksAndHazards?: string;

  @prop()
  additionalSolutionInformation?: string;

  @prop()
  @required()
  drifProgramGoalAlignment?: string;

  @prop()
  @required()
  rationaleForFunding?: string;

  @prop()
  @required()
  rationaleForSolution?: string;

  @prop()
  @required()
  estimatedPeopleImpacted?: string;

  @prop()
  // TODO: investigate why this is not working
  // @required({
  //   conditionalExpression: 'control => control.remainingAmount > 0',
  // })
  intendToSecureFunding?: string;

  @prop()
  @required()
  relatedHazards?: Hazards[];

  @prop()
  @required()
  startDate?: string;

  @propObject(ContactDetailsForm)
  submitter?: ContactDetailsForm = new ContactDetailsForm({});

  @propObject(ContactDetailsForm)
  projectContact?: ContactDetailsForm = new ContactDetailsForm({});

  @prop()
  remainingAmount?: number;

  @prop()
  units?: string;

  @prop()
  otherHazardsDescription?: string;

  @prop()
  @required()
  cfoConfirmation?: boolean;

  @prop()
  @required()
  foippaConfirmation?: boolean;

  @prop()
  @required()
  identityConfirmation?: boolean;

  @prop()
  sameAsSubmitter?: boolean;

  @prop()
  @required()
  scopeStatement?: string;

  @prop()
  @required()
  communityImpact?: string;
}
