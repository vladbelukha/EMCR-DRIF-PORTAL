import { prop } from '@rxweb/reactive-form-validators';
import { YesNoOption } from '../../../../model';
import { WorkplanProgressType } from '../../../../model/project';

export class ProgressReportForm {
  @prop()
  projectProgress?: WorkplanProgressType;

  @prop()
  projectProgressComment?: string;

  @prop()
  projectProgressDate?: string;

  @prop()
  firstNationEngagementProgress?: WorkplanProgressType;

  @prop()
  firstNationEngagementProgressComment?: string;

  @prop()
  firstNationEngagementProgressDate?: string;

  @prop()
  designProgress?: WorkplanProgressType;

  @prop()
  designProgressComment?: string;

  @prop()
  designProgressDate?: string;

  @prop()
  constructionTenderProgress?: WorkplanProgressType;

  @prop()
  constructionTenderProgressComment?: string;

  @prop()
  constructionTenderProgressDate?: string;

  @prop()
  constractionContractProgress?: WorkplanProgressType;

  @prop()
  constractionContractProgressComment?: string;

  @prop()
  constractionContractProgressDate?: string;

  @prop()
  permitToConstructProgress?: WorkplanProgressType;

  @prop()
  permitToConstructProgressComment?: string;

  @prop()
  permitToConstructProgressDate?: string;

  @prop()
  constructionProgress?: WorkplanProgressType;

  @prop()
  constructionProgressComment?: string;

  @prop()
  constructionProgressDate?: string;

  // TODO: 3 fields for construction progress?

  @prop()
  projectCompletionPercentage?: number;

  @prop()
  communityMedia?: YesNoOption; // TODO: use type

  @prop()
  communityMediaDate?: string;

  @prop()
  communityMediaComment?: string;

  @prop()
  provincialMedia?: string; // TODO: use type

  @prop()
  provincialMediaDate?: string;

  @prop()
  provincialMediaComment?: string;

  @prop()
  worksCompleted?: string;

  @prop()
  outstandingIssues?: string;

  @prop()
  fundingSourcesChanged?: boolean;

  @prop()
  fundingSourcesChangedComment?: string;
}
