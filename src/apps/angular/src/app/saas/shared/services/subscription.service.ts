import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { SubscriptionDto, CreateSubscriptionDto, UpdateSubscriptionDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  getList(input: any): Observable<any> {
    return this.restService.request<any, any>({
      method: 'GET',
      url: '/api/saas/subscriptions',
      params: input
    },
    { apiName: this.apiName });
  }

  get(id: string): Observable<SubscriptionDto> {
    return this.restService.request<any, SubscriptionDto>({
      method: 'GET',
      url: `/api/saas/subscriptions/${id}`
    },
    { apiName: this.apiName });
  }

  getCurrentTenant(): Observable<SubscriptionDto> {
    return this.restService.request<any, SubscriptionDto>({
      method: 'GET',
      url: '/api/saas/subscriptions/current-tenant'
    },
    { apiName: this.apiName });
  }

  create(input: CreateSubscriptionDto): Observable<SubscriptionDto> {
    return this.restService.request<any, SubscriptionDto>({
      method: 'POST',
      url: '/api/saas/subscriptions',
      body: input
    },
    { apiName: this.apiName });
  }

  update(id: string, input: UpdateSubscriptionDto): Observable<SubscriptionDto> {
    return this.restService.request<any, SubscriptionDto>({
      method: 'PUT',
      url: `/api/saas/subscriptions/${id}`,
      body: input
    },
    { apiName: this.apiName });
  }

  renew(id: string): Observable<SubscriptionDto> {
    return this.restService.request<any, SubscriptionDto>({
      method: 'POST',
      url: `/api/saas/subscriptions/${id}/renew`
    },
    { apiName: this.apiName });
  }

  cancel(id: string): Observable<void> {
    return this.restService.request<any, void>({
      method: 'POST',
      url: `/api/saas/subscriptions/${id}/cancel`
    },
    { apiName: this.apiName });
  }
}
