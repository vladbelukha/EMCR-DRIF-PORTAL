import { prop, propArray } from '@rxweb/reactive-form-validators';

export class InvoiceForm {
  @prop()
  invoiceNumber?: string;

  @prop()
  invoiceDate?: string;

  @prop()
  startDate?: string;

  @prop()
  endDate?: string;

  @prop()
  paymentDate?: string;

  @prop()
  claimCategory?: string; // TODO: enum

  @prop()
  supplierName?: string;

  @prop()
  description?: string;

  @prop()
  grossAmount?: number;

  @prop()
  taxRebate?: number;

  @prop()
  claimAmount?: number;

  @prop()
  pstPaid?: number;

  @prop()
  gstPaid?: number;

  constructor(value: InvoiceForm) {
    Object.assign(this, value);
  }
}

export class ClaimForm {
  @propArray(InvoiceForm)
  invoices: InvoiceForm[] = [];
}
