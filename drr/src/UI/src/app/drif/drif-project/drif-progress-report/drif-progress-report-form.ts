import {
  maxNumber,
  minNumber,
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import {
  ActivityType,
  DelayReason,
  FundingSignage,
  ProgressReport,
  ProjectProgress,
  SignageType,
  Workplan,
  WorkplanActivity,
  WorkplanStatus,
  YesNoOption,
} from '../../../../model';

import { ContactDetailsForm } from '../../drif-eoi/drif-eoi-form';

export enum EventProgressType {
  NotStarted = 'Not Started',
  InProgress = 'In Progress',
  Completed = 'Completed',
}

export class WorkplanActivityForm implements WorkplanActivity {
  @prop()
  id?: string;

  @prop()
  @required()
  activity?: ActivityType;

  @prop()
  preCreatedActivity?: boolean;

  @prop()
  isMandatory?: boolean;

  @prop()
  @required()
  status?: WorkplanStatus;

  @prop()
  // @required({
  //   conditionalExpression: function (
  //     model: any,
  //     form: any,
  //     control: any
  //   ) {
  //      TODO: this is working, but control doesn't update its state, need to investigate further
  //     console.log('model: ', model);
  //     console.log('form: ', form);
  //     console.log('control: ', control);
  //     return model.status === WorkplanProgressType.NotStarted;
  //   },
  // })
  comment?: string;

  @prop()
  @required()
  plannedStartDate?: string;

  @prop()
  @required()
  plannedCompletionDate?: string;

  @prop()
  @required()
  actualStartDate?: string;

  @prop()
  @required()
  actualCompletionDate?: string;

  constructor(values: WorkplanActivityForm) {
    Object.assign(this, values);
  }
}

export class FundingSignageForm implements FundingSignage {
  @prop()
  id?: string;

  @prop()
  @required()
  signageType?: SignageType | undefined;

  @prop()
  @required()
  beenApproved?: boolean;

  @prop()
  dateInstalled?: string;

  @prop()
  dateRemoved?: string;

  constructor(values: FundingSignageForm) {
    Object.assign(this, values);
  }
}

export class WorkplanForm implements Workplan {
  @prop()
  @required()
  projectProgress?: ProjectProgress;

  @prop()
  @required({
    conditionalExpression: function (control: any) {
      return control.projectProgress == ProjectProgress.BehindSchedule;
    },
  })
  delayReason?: DelayReason;

  @prop()
  otherDelayReason?: string;

  // TODO: the reason why this is not working with hasValidator(Validators.required)
  // is because rxweb create a custom validator that doesn't match
  // it is possible unsolvable, need to investigate further
  @required({
    conditionalExpression: function (model: any) {
      return model.projectProgress == ProjectProgress.BehindSchedule;
    },
  })
  @prop()
  behindScheduleMitigatingComments?: string;

  @prop()
  @required({
    conditionalExpression: function (control: any) {
      return control.projectProgress == ProjectProgress.AheadOfSchedule;
    },
  })
  aheadOfScheduleComments?: string;

  @propArray(WorkplanActivityForm)
  workplanActivities?: WorkplanActivityForm[] = [];

  @prop()
  @required()
  @minNumber({ value: 0 })
  @maxNumber({ value: 100 })
  projectCompletionPercentage?: number;

  @prop()
  @required()
  mediaAnnouncement?: boolean;

  @prop()
  mediaAnnouncementDate?: string;

  @prop()
  mediaAnnouncementComment?: string;

  @prop()
  worksCompleted?: string;

  @prop()
  @required()
  outstandingIssues?: boolean;

  @prop()
  outstandingIssuesComments?: string;

  @prop()
  @required()
  fundingSourcesChanged?: boolean;

  @prop()
  fundingSourcesChangedComment?: string;

  @prop()
  @required()
  signageRequired?: boolean;

  @propArray(FundingSignageForm)
  fundingSignage?: FundingSignageForm[] = [{}];

  @prop()
  signageNotRequiredComments?: string;

  constructor(values: WorkplanForm) {
    Object.assign(this, values);
  }
}

export class EventForm {
  @prop()
  groundBreaking?: EventProgressType;

  @prop()
  groundBreakingDate?: string;

  @prop()
  ribbonCutting?: EventProgressType;

  @prop()
  ribbonCuttingDate?: string;

  @prop()
  communityEngagement?: EventProgressType;

  @prop()
  communityEngagementDate?: string;

  // TODO: other events

  @prop()
  eventContact?: ContactDetailsForm;

  @prop()
  provincialRepresentativeRequest?: YesNoOption;

  @prop()
  provincialRepresentativeRequestComment?: string;

  constructor(values: EventForm) {
    Object.assign(this, values);
  }
}

export class ProgressReportForm implements ProgressReport {
  @propObject(WorkplanForm)
  workplan?: WorkplanForm = new WorkplanForm({});

  @propObject(EventForm)
  event?: EventForm = new EventForm({});
}
