import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TenantProvisioningService, EditionService } from '../shared/services';
import { EditionDto, BillingPeriod } from '../shared/models';

@Component({
  selector: 'app-tenant-provisioning',
  templateUrl: './tenant-provisioning.component.html'
})
export class TenantProvisioningComponent implements OnInit {
  form!: FormGroup;
  editions: EditionDto[] = [];
  billingPeriods = [
    { value: BillingPeriod.Monthly, label: 'Monthly' },
    { value: BillingPeriod.Quarterly, label: 'Quarterly' },
    { value: BillingPeriod.Yearly, label: 'Yearly' }
  ];
  loading = false;
  currentStep = 1;

  constructor(
    private fb: FormBuilder,
    private provisioningService: TenantProvisioningService,
    private editionService: EditionService,
    private router: Router
  ) {}

  ngOnInit() {
    this.buildForm();
    this.loadEditions();
  }

  buildForm() {
    this.form = this.fb.group({
      tenantName: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      adminEmail: ['', [Validators.required, Validators.email]],
      adminPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      editionId: ['', [Validators.required]],
      billingPeriod: [BillingPeriod.Monthly, [Validators.required]]
    });
  }

  loadEditions() {
    this.editionService.getList({ maxResultCount: 100 }).subscribe(result => {
      this.editions = result.items.filter((e: EditionDto) => e.isActive);
    });
  }

  nextStep() {
    if (this.currentStep === 1) {
      if (this.form.get('tenantName')?.valid && 
          this.form.get('adminEmail')?.valid && 
          this.form.get('adminPassword')?.valid && 
          this.form.get('confirmPassword')?.valid) {
        
        if (this.form.value.adminPassword !== this.form.value.confirmPassword) {
          alert('Passwords do not match');
          return;
        }
        this.currentStep = 2;
      }
    }
  }

  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  submitForm() {
    if (this.form.invalid) return;

    if (this.form.value.adminPassword !== this.form.value.confirmPassword) {
      alert('Passwords do not match');
      return;
    }

    this.loading = true;
    const { confirmPassword, ...formValue } = this.form.value;

    this.provisioningService.provision(formValue).subscribe({
      next: (result) => {
        alert(`Tenant ${result.tenantName} provisioned successfully!`);
        this.router.navigate(['/saas/host-admin']);
      },
      error: (err) => {
        this.loading = false;
        alert('Error provisioning tenant: ' + (err.error?.error?.message || 'Unknown error'));
      }
    });
  }

  getSelectedEdition(): EditionDto | undefined {
    return this.editions.find(e => e.id === this.form.value.editionId);
  }

  calculatePrice(): number {
    const edition = this.getSelectedEdition();
    if (!edition) return 0;

    const period = this.form.value.billingPeriod;
    if (period === BillingPeriod.Monthly) return edition.monthlyPrice;
    if (period === BillingPeriod.Yearly) return edition.yearlyPrice;
    if (period === BillingPeriod.Quarterly) return edition.monthlyPrice * 3;
    return 0;
  }
}
