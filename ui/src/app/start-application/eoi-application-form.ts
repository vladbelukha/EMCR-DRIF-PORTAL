import { prop, propArray, propObject, required } from "@rxweb/reactive-form-validators";
import { ApplicantType, ContactDetails, EOIApplication, ProjectType } from "../../model";

export class ContactDetailsForm implements ContactDetails {
    @prop()
    email?: string;
    @prop()
    firstName?: string;
    @prop()
    lastName?: string;
    @prop()
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
    
    @prop()
    otherFunding?: string[];
    
    @prop()
    otherInformation?: string;  
    
    @prop()
    ownership?: string;
    
    @prop()
    ownershipDeclaration?: boolean;
    @propArray(ContactDetailsForm)
    projectContacts?: ContactDetailsForm[];
    
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
    relatedHazards?: string[];
    
    @prop()
    startDate?: string;
    @propObject(ContactDetailsForm)
    submitter?: ContactDetailsForm = new ContactDetailsForm({});
    
    @prop()
    totalFunding?: number;
    
    @prop()
    unfundedAmount?: number;  
    
    @prop()
    units?: string;  
  }