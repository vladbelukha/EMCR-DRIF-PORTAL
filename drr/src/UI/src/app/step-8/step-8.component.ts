import { CommonModule } from '@angular/common';
import { Component, Input, inject, isDevMode } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslocoModule } from '@ngneat/transloco';
import { IFormGroup } from '@rxweb/reactive-form-validators';
import { RecaptchaFormsModule, RecaptchaModule } from 'ng-recaptcha';
import { DrifapplicationService } from '../../api/drifapplication/drifapplication.service';
import {
  DeclarationForm,
  EOIApplicationForm,
} from '../eoi-application/eoi-application-form';
import { SummaryComponent } from '../summary/summary.component';

@Component({
  selector: 'drr-step-8',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    SummaryComponent,
    TranslocoModule,
    RecaptchaModule,
    RecaptchaFormsModule,
  ],
  templateUrl: './step-8.component.html',
  styleUrl: './step-8.component.scss',
})
export class Step8Component {
  drifAppService = inject(DrifapplicationService);

  isDevMode = isDevMode();
  private _formGroup!: IFormGroup<EOIApplicationForm>;

  @Input()
  set eoiApplicationForm(eoiApplicationForm: IFormGroup<EOIApplicationForm>) {
    this._formGroup = eoiApplicationForm;
    this.declarationForm = eoiApplicationForm.get(
      'declaration'
    ) as IFormGroup<DeclarationForm>;
  }

  authorizedRepresentativeText?: string;
  accuracyOfInformationText?: string;

  ngOnInit() {
    // this.drifAppService
    //   .dRIFApplicationGetDeclarations()
    //   .subscribe((declarations) => {
    //     this.authorizedRepresentativeText = declarations.items?.find(
    //       (d) => d.type === DeclarationType.AuthorizedRepresentative
    //     )?.text;
    //     this.accuracyOfInformationText = declarations.items?.find(
    //       (d) => d.type === DeclarationType.AccuracyOfInformation
    //     )?.text;
    //   });
  }

  get eoiApplicationForm(): IFormGroup<EOIApplicationForm> {
    return this._formGroup;
  }

  declarationForm!: IFormGroup<DeclarationForm>;

  get siteKey(): string {
    // Use a test key for development and a actual key for other environments
    return this.isDevMode
      ? '6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI'
      : '6LeTReUpAAAAAKgQVQrUEP6-1rpbjFcfg-kCFY-m';
  }
}
