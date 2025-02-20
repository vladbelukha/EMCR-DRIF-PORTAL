import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {
  MatStepperModule,
  StepperOrientation,
} from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoModule, TranslocoService } from '@ngneat/transloco';
import { IFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';

import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { ProjectService } from '../../../../../api/project/project.service';
import { PeriodType } from '../../../../../model';
import { DrrDatepickerComponent } from '../../../../shared/controls/drr-datepicker/drr-datepicker.component';
import {
  DrrSelectComponent,
  DrrSelectOption,
} from '../../../../shared/controls/drr-select/drr-select.component';
import {
  InterimReportConfigurationForm,
  InterimReportForm,
} from '../drif-interim-report-form';

@Component({
  selector: 'drr-drif-interim-report-create',
  standalone: true,
  imports: [
    CommonModule,
    MatStepperModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    TranslocoModule,
    DrrDatepickerComponent,
    DrrSelectComponent,
  ],
  templateUrl: './drif-interim-report-create.component.html',
  styleUrl: './drif-interim-report-create.component.scss',
  providers: [RxFormBuilder],
})
export class DrifInterimReportCreateComponent {
  formBuilder = inject(RxFormBuilder);
  translocoService = inject(TranslocoService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  projectService = inject(ProjectService);

  projectId?: string;

  stepperOrientation: StepperOrientation = 'horizontal';

  interimReportForm = this.formBuilder.formGroup(
    InterimReportForm,
  ) as IFormGroup<InterimReportForm>;

  periodTypeOptions?: DrrSelectOption[] = Object.keys(PeriodType).map(
    (value) => {
      return {
        value,
        label: this.translocoService.translate(`periodType.${value}`),
      };
    },
  );

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.projectId = params['projectId'];
    });
  }

  stepperSelectionChange(event: StepperSelectionEvent) {
    switch (event.selectedIndex) {
      case 1:
        this.canCreateReport();
        break;
      case 2:
        this.createReport();
        break;

      default:
        break;
    }
  }

  getConfigurationForm() {
    return this.interimReportForm.get(
      'configuration',
    ) as IFormGroup<InterimReportConfigurationForm>;
  }

  canCreateReport() {
    this.projectService
      .projectValidateCanCreateReport(this.projectId!, {
        reportType: this.interimReportForm.value.configuration?.periodType,
      })
      .subscribe((response) => {
        console.log(response);
      });
  }

  createReport() {}

  goBack() {
    this.router.navigate(['drif-projects', this.projectId]);
  }
}
