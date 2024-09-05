import { prop } from '@rxweb/reactive-form-validators';
import { ScreenerQuestions, YesNoOption } from '../../../../model';

export class ScreenerQuestionsForm implements ScreenerQuestions {
  @prop()
  costEstimate?: boolean | undefined;

  @prop()
  engagedWithFirstNationsOccurred?: boolean | undefined;

  @prop()
  firstNationsAuthorizedByPartners?: YesNoOption | undefined;

  @prop()
  foundationWorkCompleted?: YesNoOption | undefined;

  @prop()
  haveAuthorityToDevelop?: boolean | undefined;

  @prop()
  incorporateFutureClimateConditions?: boolean | undefined;

  @prop()
  localGovernmentAuthorizedByPartners?: YesNoOption | undefined;

  @prop()
  meetsEligibilityRequirements?: boolean | undefined;

  @prop()
  projectSchedule?: boolean | undefined;

  @prop()
  projectWorkplan?: boolean | undefined;

  @prop()
  sitePlan?: YesNoOption | undefined;

  @prop()
  meetsRegulatoryRequirements?: boolean | undefined;

  constructor(values: ScreenerQuestionsForm) {
    Object.assign(this, values);
  }
}
