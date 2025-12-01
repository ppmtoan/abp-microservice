import { Component, OnInit } from '@angular/core';
import { HostAdminService } from '../../shared/services';
import { HostMetricsDto, TenantDetailDto } from '../../shared/models';
import { ListService, PagedResultDto } from '@abp/ng.core';

@Component({
  selector: 'app-host-dashboard',
  templateUrl: './host-dashboard.component.html',
  providers: [ListService]
})
export class HostDashboardComponent implements OnInit {
  metrics: HostMetricsDto | null = null;
  tenants = { items: [], totalCount: 0 } as PagedResultDto<TenantDetailDto>;

  constructor(
    private hostAdminService: HostAdminService,
    public readonly list: ListService
  ) {}

  ngOnInit() {
    this.loadMetrics();
    this.loadTenants();
  }

  loadMetrics() {
    this.hostAdminService.getMetrics().subscribe(data => {
      this.metrics = data;
    });
  }

  loadTenants() {
    const tenantStreamCreator = (query: any) => this.hostAdminService.getTenants(query);
    this.list.hookToQuery(tenantStreamCreator).subscribe((response) => {
      this.tenants = response as PagedResultDto<TenantDetailDto>;
    });
  }

  activateTenant(id: string) {
    this.hostAdminService.activateTenant(id).subscribe(() => {
      this.list.get();
      this.loadMetrics();
    });
  }

  deactivateTenant(id: string) {
    this.hostAdminService.deactivateTenant(id).subscribe(() => {
      this.list.get();
      this.loadMetrics();
    });
  }
}
