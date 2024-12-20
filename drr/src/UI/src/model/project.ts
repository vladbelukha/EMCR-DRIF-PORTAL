import { Attachment } from './attachment';
import { ContactDetails } from './contactDetails';
import { FundingStream } from './fundingStream';
import { ProgramType } from './programType';

// TODO: to be replaced wwith API model when availale
export interface Project {
  id: string;
  projectTitle: string;
  contractNumber: string;
  proponentName: string;
  fundingStream: FundingStream;
  projectNumber?: string;
  programType: ProgramType;
  reportingScheduleType: ReportingScheduleType;
  fundingAmount: number;
  startDate: string;
  endDate: string;
  projectStatus: string;
  contacts: ContactDetails[];
  interimReports: InterimReport[];
  claims: Claim[];
  progressReports: ProgressReport[];
  forecast: Forecast[];
  attachments: Attachment[];
}

export enum ReportingScheduleType {
  Quarterly = 'Quarterly',
  Monthly = 'Monthly',
}

export interface InterimReport {
  id: string;
  reportDate: string;
  reportStatus: string;
  claim: Claim;
  report: ProgressReport;
  forecast: Forecast;
}

export interface Claim {
  id: string;
  claimType: string;
  claimDate: string;
  claimAmount: number;
  claimStatus: string;
}

export interface ProgressReport {
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
