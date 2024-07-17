import { propObject } from '@rxweb/reactive-form-validators';
import { ProponentInformationForm } from '../drif-eoi/drif-eoi-form';

export class DrifFpForm {
  @propObject(ProponentInformationForm)
  proponentInformation?: ProponentInformationForm =
    new ProponentInformationForm({});
}
