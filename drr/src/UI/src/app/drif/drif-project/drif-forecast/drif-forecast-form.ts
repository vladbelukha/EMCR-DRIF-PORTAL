import { prop, propArray } from '@rxweb/reactive-form-validators';

export class YearForecastForm {
  @prop()
  id?: string;

  @prop()
  fiscalYear?: string;

  @prop()
  originalForecast?: number;

  @prop()
  projectedExpenditure?: number;

  @prop()
  paidClaimsAmount?: number;

  @prop()
  outstandingClaimsAmount?: number;

  @prop()
  remainingClaimsAmount?: number;

  constructor(data?: Partial<YearForecastForm>) {
    Object.assign(this, data);
  }
}

export class ForecastForm {
  @prop()
  id?: string;

  @propArray(YearForecastForm)
  yearForecasts?: YearForecastForm[] = [];
}
