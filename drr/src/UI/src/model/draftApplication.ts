/**
 * Generated by orval v6.27.1 🍺
 * Do not edit manually.
 * DRR API
 * OpenAPI spec version: 1.0.0
 */
import type { ContactDetails } from './contactDetails';
import type { EstimatedNumberOfPeople } from './estimatedNumberOfPeople';
import type { FundingStream } from './fundingStream';
import type { FundingInformation } from './fundingInformation';
import type { ProjectType } from './projectType';
import type { ProponentType } from './proponentType';
import type { Hazards } from './hazards';
import type { SubmissionPortalStatus } from './submissionPortalStatus';

export interface DraftApplication {
  /** @nullable */
  additionalBackgroundInformation?: string;
  additionalContacts?: ContactDetails[];
  /** @nullable */
  additionalEngagementInformation?: string;
  /** @nullable */
  additionalSolutionInformation?: string;
  /** @nullable */
  addressRisksAndHazards?: string;
  /** @nullable */
  climateAdaptation?: string;
  /** @nullable */
  communityImpact?: string;
  /** @nullable */
  disasterRiskUnderstanding?: string;
  /** @nullable */
  drifProgramGoalAlignment?: string;
  /** @nullable */
  endDate?: string;
  /** @nullable */
  estimatedPeopleImpacted?: EstimatedNumberOfPeople;
  /**
   * @minimum 0
   * @maximum 999999999.99
   * @nullable
   */
  estimatedTotal?: number;
  /** @nullable */
  firstNationsEngagement?: string;
  /**
   * @minimum 0
   * @maximum 999999999.99
   * @nullable
   */
  fundingRequest?: number;
  /** @nullable */
  fundingStream?: FundingStream;
  /** @nullable */
  id?: string;
  infrastructureImpacted?: string[];
  /** @nullable */
  intendToSecureFunding?: string;
  /** @nullable */
  locationDescription?: string;
  /** @nullable */
  neighbourEngagement?: string;
  otherFunding?: FundingInformation[];
  /** @nullable */
  otherHazardsDescription?: string;
  /** @nullable */
  otherInformation?: string;
  /** @nullable */
  ownershipDeclaration?: boolean;
  /** @nullable */
  ownershipDescription?: string;
  partneringProponents?: string[];
  /** @nullable */
  projectContact?: ContactDetails;
  /** @nullable */
  projectTitle?: string;
  /** @nullable */
  projectType?: ProjectType;
  /** @nullable */
  proponentType?: ProponentType;
  /** @nullable */
  rationaleForFunding?: string;
  /** @nullable */
  rationaleForSolution?: string;
  /** @nullable */
  relatedHazards?: Hazards[];
  /** @nullable */
  remainingAmount?: number;
  /** @nullable */
  scopeStatement?: string;
  /** @nullable */
  startDate?: string;
  /** @nullable */
  status?: SubmissionPortalStatus;
  /** @nullable */
  submitter?: ContactDetails;
}