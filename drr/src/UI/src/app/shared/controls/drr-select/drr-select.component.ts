import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { TranslocoModule } from '@ngneat/transloco';
import { UntilDestroy } from '@ngneat/until-destroy';
import { RxFormBuilder, RxFormControl } from '@rxweb/reactive-form-validators';

@UntilDestroy({ checkProperties: true })
@Component({
  selector: 'drr-select',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatSelectModule,
    TranslocoModule,
  ],
  templateUrl: './drr-select.component.html',
  styleUrl: './drr-select.component.scss',
})
export class DrrSelectComponent {
  formBuilder = inject(RxFormBuilder);
  breakpointObserver = inject(BreakpointObserver);

  isMobile = false;

  private _formControl = this.formBuilder.control('', []) as RxFormControl;
  @Input()
  set rxFormControl(rxFormControl: any) {
    this._formControl = rxFormControl as RxFormControl;
  }
  get rxFormControl() {
    return this._formControl;
  }

  @Input() isMultiple = false;
  @Input() label = '';
  @Input() id = '';
  @Input() options: string[] = [];

  @Output()
  selectionChange = new EventEmitter<MatSelectChange>();

  ngOnInit() {
    this.breakpointObserver
      .observe('(min-width: 768px)')
      .subscribe(({ matches }) => {
        this.isMobile = !matches;
      });
  }

  onSelectionChange(event: MatSelectChange) {
    this.selectionChange.emit(event);
  }

  getMandatoryMark() {
    return !!this.rxFormControl?.validator?.({})?.required ? '*' : '';
  }

  isRequired(): boolean {
    return (
      !!this.rxFormControl?.validator?.({})?.required ||
      !!this.rxFormControl?.validator?.({})?.minLength
    );
  }
}
