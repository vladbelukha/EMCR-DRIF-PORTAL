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
  ApplicantType,
  ContactDetails,
  DrifEoiApplication,
  FundingInformation,
  FundingType,
  Hazards,
  LocationInformation,
  ProjectType,
} from '../../model';

export class FundingInformationForm implements FundingInformation {
  @prop()
  amount?: number;
  @prop()
  name?: string;
  @prop()
  type?: FundingType;

  constructor(values: FundingInformationForm) {
    Object.assign(this, values);
  }
}

export class LocationInformationForm implements LocationInformation {
  @prop()
  @required()
  area?: string;

  @prop()
  description?: string;

  @prop()
  ownership?: string;

  constructor(values: LocationInformationForm) {
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
  applicantName?: string;

  @prop()
  @required()
  applicantType?: ApplicantType;

  @prop()
  area?: number;

  @prop()
  @required()
  backgroundDescription?: string;

  @prop()
  @required()
  climateAdaptation?: string;

  @prop()
  coordinates?: string;

  @prop()
  @required()
  endDate?: string;

  @prop()
  @required()
  engagementProposal?: string;

  @prop()
  @required()
  fundingRequest?: number;

  @propObject(LocationInformationForm)
  locationInformation?: LocationInformationForm = new LocationInformationForm(
    {}
  );

  @propArray(FundingInformationForm)
  otherFunding?: FundingInformationForm[] = [];

  @prop()
  otherInformation?: string;

  @prop()
  @required()
  ownershipDeclaration?: boolean;

  @propArray(ContactDetailsForm)
  // TODO: minItems(1)
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
  proposedSolution?: string;

  @prop()
  @required()
  rationaleForFunding?: string;

  @prop()
  @required()
  rationaleForSolution?: string;

  @prop()
  reasonsToSecureFunding?: string;

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
  totalFunding?: number;

  @prop()
  unfundedAmount?: number;

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
}
