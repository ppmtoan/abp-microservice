import { Component, EventEmitter, Input, OnInit, OnChanges, SimpleChanges, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EditionDto, CreateEditionDto, UpdateEditionDto } from '../../shared/models';
import { EditionService } from '../../shared/services';

@Component({
  selector: 'app-edition-form',
  templateUrl: './edition-form.component.html'
})
export class EditionFormComponent implements OnInit, OnChanges {
  @Input() visible = false;
  @Input() edition: EditionDto | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter();

  form!: FormGroup;
  featureLimitsJson = '{}';

  constructor(
    private fb: FormBuilder,
    private editionService: EditionService
  ) {}

  ngOnInit() {
    this.buildForm();
  }

  ngOnChanges() {
    if (this.edition) {
      this.form?.patchValue({
        name: this.edition.name,
        displayName: this.edition.displayName,
        monthlyPrice: this.edition.monthlyPrice,
        yearlyPrice: this.edition.yearlyPrice,
        isActive: this.edition.isActive
      });
      this.featureLimitsJson = JSON.stringify(this.edition.featureLimits || {}, null, 2);
    } else {
      this.form?.reset({
        isActive: true
      });
      this.featureLimitsJson = JSON.stringify({
        MaxUsers: 10,
        MaxProjects: 10,
        StorageGB: 10
      }, null, 2);
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: [null, [Validators.required, Validators.maxLength(50)]],
      displayName: [null, [Validators.required, Validators.maxLength(100)]],
      monthlyPrice: [0, [Validators.required, Validators.min(0)]],
      yearlyPrice: [0, [Validators.required, Validators.min(0)]],
      isActive: [true]
    });
  }

  submitForm() {
    if (this.form.invalid) return;

    let featureLimits = {};
    try {
      featureLimits = JSON.parse(this.featureLimitsJson);
    } catch (e) {
      alert('Invalid JSON for feature limits');
      return;
    }

    const formValue = {
      ...this.form.value,
      featureLimits
    };

    const request$ = this.edition
      ? this.editionService.update(this.edition.id, formValue as UpdateEditionDto)
      : this.editionService.create(formValue as CreateEditionDto);

    request$.subscribe(() => {
      this.visible = false;
      this.visibleChange.emit(false);
      this.save.emit();
      this.form.reset();
    });
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
    this.form.reset();
  }
}
