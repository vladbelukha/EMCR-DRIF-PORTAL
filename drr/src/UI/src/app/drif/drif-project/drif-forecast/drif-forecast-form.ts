import { AbstractControl } from '@angular/forms';
import { disable, prop, propArray } from '@rxweb/reactive-form-validators';

export class YearForecastForm {
  @prop()
  id?: string;

  @prop()
  fiscalYear?: string;

  @prop()
  @disable({
    conditionalExpression: function (control: AbstractControl) {
      return true;
    },
  })
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
