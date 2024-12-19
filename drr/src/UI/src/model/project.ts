import { Attachment } from './attachment';
import { ContactDetails } from './contactDetails';
import { FundingStream } from './fundingStream';
import { ProgramType } from './programType';

// TODO: to be replaced wwith API model when availale
export interface Project {
  id: string;
  projectTitle: string;
  proponentName: string;
  fundingStream: FundingStream;
  projectNumber?: string;
  programType: ProgramType;
  projectStatus: string;
  contacts: ContactDetails[];
  claims: Claim[];
  reports: Report[];
  forecast: Forecast[];
  attachments: Attachment[];
}

export interface Claim {
  id: string;
  claimType: string;
  claimDate: string;
  claimAmount: number;
  claimStatus: string;
}

export interface Report {
  id: string;
  reportType: string;
  reportDate: string;
  reportStatus: string;
}

export interface Forecast {
  id: string;
  forecastType: string;
  forecastDate: string;
  forecastAmount: number;
  forecastStatus: string;
}
