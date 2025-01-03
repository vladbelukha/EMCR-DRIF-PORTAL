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
  eoiId: string;
  fpId: string;
  status: ProjectStatus;
  contacts: ContactDetails[];
  interimReports: InterimReport[];
  claims: Claim[];
  progressReports: ProgressReport[];
  forecast: Forecast[];
  attachments: Attachment[];
}

export enum ProjectStatus {
  Active = 'Active',
  Inactive = 'Inactive',
}

export enum ReportingScheduleType {
  Quarterly = 'Quarterly',
  Monthly = 'Monthly',
}

export interface InterimReport {
  id: string;
  dueDate: string;
  description?: string;
  status: InterimReportStatus;
  claim?: Claim;
  report?: ProgressReport;
  forecast?: Forecast;
}

export enum WorkplanProgressType {
  NotStarted = 'Not Started',
  InProgress = 'In Progress',
  Completed = 'Completed',
  NotApplicable = 'Not Applicable',
}

export enum EventProgressType {
  NotPlanned = 'Not Planned',
  PlannedDateUnknown = 'Planned Date Unknown',
  PlannedDateKnown = 'Planned Date Known',
  AlreadyOccurred = 'Already Occurred',
  Unknown = 'Unknown',
}

export enum InterimReportStatus {
  Pending = 'Pending',
  Review = 'Review',
  Approved = 'Approved',
  Rejected = 'Rejected',
}

export interface Claim {
  id: string;
  claimType: string;
  claimDate: string;
  claimAmount: number;
  status: ClaimStatus;
}

export enum ClaimStatus {
  Pending = 'Pending',
  Review = 'Review',
  Approved = 'Approved',
  Rejected = 'Rejected',
}

export interface ProgressReport {
  id: string;
  reportType: string;
  reportDate: string;
  status: ProgressReportStatus;
}

export enum ProgressReportStatus {
  Pending = 'Pending',
  Review = 'Review',
  Approved = 'Approved',
  Rejected = 'Rejected',
}

export interface Forecast {
  id: string;
  forecastType: string;
  forecastDate: string;
  forecastAmount: number;
  status: ForecastStatus;
}

export enum ForecastStatus {
  Pending = 'Pending',
  Review = 'Review',
  Approved = 'Approved',
  Rejected = 'Rejected',
}
