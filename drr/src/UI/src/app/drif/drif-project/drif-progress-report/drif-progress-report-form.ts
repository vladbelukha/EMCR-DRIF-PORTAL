import {
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import {
  ActivityType,
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

export class WorkplanForm {
  @propArray(WorkplanActivityForm)
  workplanActivities?: WorkplanActivityForm[] = [];

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

export class ProgressReportForm {
  @propObject(WorkplanForm)
  workplan?: WorkplanForm = new WorkplanForm({});

  @propObject(EventForm)
  event?: EventForm = new EventForm({});
}
