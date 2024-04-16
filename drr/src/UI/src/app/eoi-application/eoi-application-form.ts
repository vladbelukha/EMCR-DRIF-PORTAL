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
  AreaUnits,
  ContactDetails,
  EOIApplication,
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
  area?: string;

  @prop()
  areaUnits?: AreaUnits;

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
  position?: string; // TODO: use as department for now

  @prop()
  title?: string;

  constructor(values: ContactDetailsForm) {
    Object.assign(this, values);
  }
}

export class EOIApplicationForm implements EOIApplication {
  @prop()
  @required()
  applicantName?: string;

  @prop()
  @required()
  applicantType?: ApplicantType;

  @prop()
  area?: number;

  @prop()
  backgroundDescription?: string;

  @prop()
  cfoConfirmation?: boolean;

  @prop()
  climateAdaptation?: string;

  @prop()
  coordinates?: string;

  @prop()
  endDate?: string;

  @prop()
  engagementProposal?: string;

  @prop()
  foippaConfirmation?: boolean;

  @prop()
  fundingRequest?: number;

  @prop()
  identityConfirmation?: boolean;

  @prop()
  locationDescription?: string;

  @propObject(LocationInformationForm)
  locationInformation?: LocationInformationForm = new LocationInformationForm(
    {}
  );

  @propArray(FundingInformationForm)
  otherFunding?: FundingInformationForm[] = [{}];

  @prop()
  otherInformation?: string;

  @prop()
  ownership?: string;

  @prop()
  ownershipDeclaration?: boolean;

  @propArray(ContactDetailsForm)
  projectContacts?: ContactDetailsForm[] = [{}];

  @prop()
  projectTitle?: string;

  @prop()
  projectType?: ProjectType;

  @prop()
  proposedSolution?: string;

  @prop()
  rationaleForFunding?: string;

  @prop()
  rationaleForSolution?: string;

  @prop()
  reasonsToSecureFunding?: string;

  @prop()
  relatedHazards?: Hazards[];

  @prop()
  startDate?: string;

  @propObject(ContactDetailsForm)
  submitter?: ContactDetailsForm = new ContactDetailsForm({});

  @prop()
  @disable({
    conditionalExpression: () => {
      return true;
    },
  })
  totalFunding?: number;

  @prop()
  unfundedAmount?: number;

  @prop()
  units?: string;

  @prop()
  otherHazardsDescription?: string;
}
