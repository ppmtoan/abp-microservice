import { Component, OnInit } from '@angular/core';
import { TenantAdminService, InvoiceService } from '../../shared/services';
import { TenantDashboardDto } from '../../shared/models';

@Component({
  selector: 'app-tenant-dashboard',
  templateUrl: './tenant-dashboard.component.html'
})
export class TenantDashboardComponent implements OnInit {
  dashboard: TenantDashboardDto | null = null;
  loading = true;

  constructor(
    private tenantAdminService: TenantAdminService
  ) {}

  ngOnInit() {
    this.loadDashboard();
  }

  loadDashboard() {
    this.loading = true;
    this.tenantAdminService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getFeatureLimitKeys(): string[] {
    return this.dashboard?.featureLimits ? Object.keys(this.dashboard.featureLimits) : [];
  }
}
