import { prop, required } from '@rxweb/reactive-form-validators';
import { PeriodType } from '../../../../model';

export class InterimReportForm {
  @prop()
  @required()
  periodType?: PeriodType;

  @prop()
  createDate?: Date;

  @prop()
  dueDate?: Date;

  @prop()
  year?: number;

  @prop()
  quarter?: string;

  @prop()
  description?: string;
}
