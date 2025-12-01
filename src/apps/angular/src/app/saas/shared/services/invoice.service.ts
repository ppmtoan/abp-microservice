import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { InvoiceDto, MarkInvoiceAsPaidDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  getList(input: any): Observable<any> {
    return this.restService.request<any, any>({
      method: 'GET',
      url: '/api/saas/invoices',
      params: input
    },
    { apiName: this.apiName });
  }

  get(id: string): Observable<InvoiceDto> {
    return this.restService.request<any, InvoiceDto>({
      method: 'GET',
      url: `/api/saas/invoices/${id}`
    },
    { apiName: this.apiName });
  }

  getCurrentTenantInvoices(): Observable<InvoiceDto[]> {
    return this.restService.request<any, InvoiceDto[]>({
      method: 'GET',
      url: '/api/saas/invoices/current-tenant'
    },
    { apiName: this.apiName });
  }

  markAsPaid(id: string, input: MarkInvoiceAsPaidDto): Observable<InvoiceDto> {
    return this.restService.request<any, InvoiceDto>({
      method: 'POST',
      url: `/api/saas/invoices/${id}/mark-as-paid`,
      body: input
    },
    { apiName: this.apiName });
  }

  processOverdue(): Observable<void> {
    return this.restService.request<any, void>({
      method: 'POST',
      url: '/api/saas/invoices/process-overdue'
    },
    { apiName: this.apiName });
  }
}
