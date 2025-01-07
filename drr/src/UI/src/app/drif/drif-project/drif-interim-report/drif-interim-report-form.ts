import { prop } from '@rxweb/reactive-form-validators';
import { InterimReportType, ReportQuarter } from '../../../../model/project';

export class InterimReportForm {
  @prop()
  type?: InterimReportType;

  @prop()
  date?: Date;

  @prop()
  dueDate?: Date;

  @prop()
  year?: number;

  @prop()
  quarter?: ReportQuarter;
}
