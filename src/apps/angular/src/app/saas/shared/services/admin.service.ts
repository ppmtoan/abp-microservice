import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { TenantProvisioningRequestDto, TenantProvisioningResultDto, HostMetricsDto, TenantDetailDto, TenantDashboardDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class TenantProvisioningService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  provision(input: TenantProvisioningRequestDto): Observable<TenantProvisioningResultDto> {
    return this.restService.request<any, TenantProvisioningResultDto>({
      method: 'POST',
      url: '/api/saas/tenant-provisioning/provision',
      body: input
    },
    { apiName: this.apiName });
  }
}

@Injectable({
  providedIn: 'root'
})
export class HostAdminService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  getMetrics(): Observable<HostMetricsDto> {
    return this.restService.request<any, HostMetricsDto>({
      method: 'GET',
      url: '/api/saas/host-admin/metrics'
    },
    { apiName: this.apiName });
  }

  getTenants(input: any): Observable<any> {
    return this.restService.request<any, any>({
      method: 'GET',
      url: '/api/saas/host-admin/tenants',
      params: input
    },
    { apiName: this.apiName });
  }

  getTenantDetail(id: string): Observable<TenantDetailDto> {
    return this.restService.request<any, TenantDetailDto>({
      method: 'GET',
      url: `/api/saas/host-admin/tenants/${id}/detail`
    },
    { apiName: this.apiName });
  }

  activateTenant(id: string): Observable<void> {
    return this.restService.request<any, void>({
      method: 'POST',
      url: `/api/saas/host-admin/tenants/${id}/activate`
    },
    { apiName: this.apiName });
  }

  deactivateTenant(id: string): Observable<void> {
    return this.restService.request<any, void>({
      method: 'POST',
      url: `/api/saas/host-admin/tenants/${id}/deactivate`
    },
    { apiName: this.apiName });
  }
}

@Injectable({
  providedIn: 'root'
})
export class TenantAdminService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  getDashboard(): Observable<TenantDashboardDto> {
    return this.restService.request<any, TenantDashboardDto>({
      method: 'GET',
      url: '/api/saas/tenant-admin/dashboard'
    },
    { apiName: this.apiName });
  }
}
