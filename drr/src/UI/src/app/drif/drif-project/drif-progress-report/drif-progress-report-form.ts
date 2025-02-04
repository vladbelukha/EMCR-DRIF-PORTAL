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
  ProgressReport,
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

// TODO: remove after API introduces this enum
export enum ProjectProgressStatus {
  OnSchedule = 'OnSchedule',
  AheadOfSchedule = 'AheadOfSchedule',
  BehindSchedule = 'BehindSchedule',
  Completed = 'Completed',
}
// TODO: remove after API introduces this enum
export enum ReasonsForDelay {
  Reason1 = 'Reason 1',
  Reason2 = 'Reason 2',
  Reason3 = 'Reason 3',
}

export class WorkplanForm implements Workplan {
  @prop()
  @required()
  projectProgressStatus?: ProjectProgressStatus;

  @prop()
  @required({
    conditionalExpression: function (control: any) {
      return (
        control.projectProgressStatus == ProjectProgressStatus.BehindSchedule
      );
    },
  })
  // @minNumber({
  //   value: 1,
  //   conditionalExpression: function (control: any) {
  //     return (
  //       control.projectProgressStatus == ProjectProgressStatus.BehindSchedule
  //     );
  //   },
  // })
  reasonsForDelay?: string[];

  @required({
    conditionalExpression: function (control: any) {
      return (
        control.projectProgressStatus == ProjectProgressStatus.BehindSchedule
      );
    },
  })
  @prop()
  reasonsForDelayComment?: string;

  @prop()
  @required({
    conditionalExpression: function (control: any) {
      return (
        control.projectProgressStatus == ProjectProgressStatus.AheadOfSchedule
      );
    },
  })
  reasonsForBeingAhead?: string;

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
  outstandingIssuesComment?: string;

  @prop()
  @required()
  fundingSourcesChanged?: boolean;

  @prop()
  fundingSourcesChangedComment?: string;

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
