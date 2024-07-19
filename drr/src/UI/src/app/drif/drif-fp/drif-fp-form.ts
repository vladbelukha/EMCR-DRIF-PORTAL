import { prop, propObject } from '@rxweb/reactive-form-validators';
import { FundingStream, ProjectType } from '../../../model';
import { ProponentInformationForm } from '../drif-eoi/drif-eoi-form';

export class DrifFpForm {
  @propObject(ProponentInformationForm)
  proponentInformation?: ProponentInformationForm =
    new ProponentInformationForm({});

  @prop()
  eoiId?: string;

  @prop()
  fundingStream?: FundingStream;

  @prop()
  projectType?: ProjectType;
  // TODO: have a factory method to create a new instance of this form
}
