import {
  email,
  prop,
  propArray,
  propObject,
  required,
  requiredTrue,
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

export class FundingInformationItemForm implements FundingInformation {
  @prop()
  amount?: number;
  @prop()
  name?: string;
  @prop()
  type?: FundingType;
  @prop()
  otherDescription?: string;

  constructor(values: FundingInformationItemForm) {
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

export class ProponentInformationForm {
  @prop()
  @required()
  proponentType?: ProponentType;

  @prop()
  @required()
  proponentName?: string;

  @required()
  @propObject(ContactDetailsForm)
  submitter?: ContactDetailsForm = new ContactDetailsForm({});

  @prop()
  sameAsSubmitter?: boolean;

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

  constructor(values: ProponentInformationForm) {
    Object.assign(this, values);
  }
}

export class ProjectInformationForm {
  @prop()
  @required()
  fundingStream?: FundingStream;

  @prop()
  @required()
  projectTitle?: string;

  @prop()
  @required()
  scopeStatement?: string;

  @prop()
  @required()
  projectType?: ProjectType;

  @prop()
  @required()
  relatedHazards?: Hazards[];

  @prop()
  otherHazardsDescription?: string;

  @prop()
  @required()
  startDate?: string;

  @prop()
  @required()
  endDate?: string;

  constructor(values: ProjectInformationForm) {
    Object.assign(this, values);
  }
}

export class FundingInformationForm {
  @prop()
  @required()
  estimatedTotal?: number;

  @prop()
  @required()
  fundingRequest?: number;

  @propArray(FundingInformationItemForm)
  otherFunding?: FundingInformationItemForm[] = [];

  @prop()
  remainingAmount?: number;

  @prop()
  @required()
  intendToSecureFunding?: string;

  constructor(values: FundingInformationForm) {
    Object.assign(this, values);
  }
}

export class LocationInformationForm {
  @prop()
  @required()
  ownershipDeclaration?: boolean;

  @prop()
  ownershipDescription?: string;

  @prop()
  @required()
  locationDescription?: string;

  constructor(values: LocationInformationForm) {
    Object.assign(this, values);
  }
}

export class ProjectDetailsForm {
  @prop()
  @required()
  rationaleForFunding?: string;

  @prop()
  @required()
  estimatedPeopleImpacted?: number;

  @prop()
  @required()
  communityImpact?: string;

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
  @required()
  addressRisksAndHazards?: string;

  @prop()
  @required()
  drifProgramGoalAlignment?: string;

  @prop()
  additionalSolutionInformation?: string;

  @prop()
  @required()
  rationaleForSolution?: string;

  constructor(values: ProjectDetailsForm) {
    Object.assign(this, values);
  }
}

export class EngagementPlanForm {
  @prop()
  @required()
  firstNationsEngagement?: string;

  @prop()
  @required()
  neighbourEngagement?: string;

  @prop()
  additionalEngagementInformation?: string;

  constructor(values: EngagementPlanForm) {
    Object.assign(this, values);
  }
}

export class OtherSupportingInformationForm {
  @prop()
  @required()
  climateAdaptation?: string;

  @prop()
  otherInformation?: string;

  constructor(values: OtherSupportingInformationForm) {
    Object.assign(this, values);
  }
}

export class EOIApplicationForm implements DrifEoiApplication {
  @propObject(ProponentInformationForm)
  proponentInformation?: ProponentInformationForm =
    new ProponentInformationForm({});

  @propObject(ProjectInformationForm)
  projectInformation?: ProjectInformationForm = new ProjectInformationForm({});

  @propObject(FundingInformationForm)
  fundingInformation?: FundingInformationForm = new FundingInformationForm({});

  @propObject(LocationInformationForm)
  locationInformation?: LocationInformationForm = new LocationInformationForm(
    {}
  );

  @propObject(ProjectDetailsForm)
  projectDetails?: ProjectDetailsForm = new ProjectDetailsForm({});

  @propObject(EngagementPlanForm)
  engagementPlan?: EngagementPlanForm = new EngagementPlanForm({});

  @propObject(OtherSupportingInformationForm)
  otherSupportingInformation?: OtherSupportingInformationForm =
    new OtherSupportingInformationForm({});

  @prop()
  @required()
  @requiredTrue()
  financialAwarenessConfirmation?: boolean;

  @prop()
  @required()
  @requiredTrue()
  foippaConfirmation?: boolean;

  @prop()
  @required()
  @requiredTrue()
  identityConfirmation?: boolean;
}
