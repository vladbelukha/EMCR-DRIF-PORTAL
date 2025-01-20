import {
  prop,
  propArray,
  propObject,
  required,
} from '@rxweb/reactive-form-validators';
import { YesNoOption } from '../../../../model';

import { ContactDetailsForm } from '../../drif-eoi/drif-eoi-form';

export enum WorkplanProgressType {
  NotStarted = 'Not Started',
  InProgress = 'In Progress',
  Completed = 'Completed',
  NotApplicable = 'Not Applicable',
}

export enum EventProgressType {
  NotStarted = 'Not Started',
  InProgress = 'In Progress',
  Completed = 'Completed',
}

export enum ProjectActivityType {
  ProjectProgress = 'Project Progress',
  FirstNationEngagement = 'First Nation Engagement',
  Design = 'Design',
  ConstructionTender = 'Construction Tender',
  ConstructionContract = 'Construction Contract',
  PermitToConstruct = 'Permit to Construct',
  Construction = 'Construction',
  Additional = 'Additional',
}

export class WorkplanItemForm {
  @prop()
  @required()
  activity?: ProjectActivityType;

  @prop()
  isPreDefinedActivity?: boolean;

  @prop()
  @required()
  status?: WorkplanProgressType;

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
  plannedEndDate?: string;

  @prop()
  @required()
  actualStartDate?: string;

  @prop()
  @required()
  actualEndDate?: string;

  constructor(values: WorkplanItemForm) {
    Object.assign(this, values);
  }
}

export class WorkplanForm {
  @propArray(WorkplanItemForm)
  workplanItems?: WorkplanItemForm[] = [];

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
