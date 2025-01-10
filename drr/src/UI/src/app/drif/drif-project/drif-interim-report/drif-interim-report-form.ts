import { prop } from '@rxweb/reactive-form-validators';

export class InterimReportForm {
  @prop()
  type?: string;

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
