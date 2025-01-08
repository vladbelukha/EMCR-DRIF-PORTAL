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
  conditions: PaymentCondition[];
  fundingAmount: number;
  startDate: string;
  endDate: string;
  eoiId: string;
  fpId: string;
  status: ProjectStatus;
  contacts: ContactDetails[];
  interimReports: InterimReport[];
  attachments: Attachment[];
  // claims: Claim[];
  // progressReports: ProgressReport[];
  // forecast: Forecast[];
}

export enum ProjectStatus {
  Active = 'Active',
  Inactive = 'Inactive',
}

export interface PaymentCondition {
  id: string;
  conditionName: string;
  limit: number;
  status: PaymentConditionStatus;
  dateMet?: string;
}

export enum PaymentConditionStatus {
  Met = 'Met',
  NotMet = 'Not Met',
}

export enum ReportingScheduleType {
  Quarterly = 'Quarterly',
  Monthly = 'Monthly',
}

export enum ReportQuarter {
  Q1 = 'Q1',
  Q2 = 'Q2',
  Q3 = 'Q3',
  Q4 = 'Q4',
}

export enum InterimReportType {
  Interim = 'Interim Report',
  OffCycle = 'Request Off-cycle payment',
  Final = 'Final',
  Skip = 'Request to skip report',
}

export interface InterimReport {
  id: string;
  dueDate: string;
  description?: string;
  type: InterimReportType;
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
  earliestInvoice?: string;
  latestInvoice?: string;
  invoiceCount?: number;
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
  Skipped = 'Skipped',
}
