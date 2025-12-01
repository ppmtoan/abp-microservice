import { Injectable } from '@angular/core';
import { RestService, Rest } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { EditionDto, CreateEditionDto, UpdateEditionDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class EditionService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  getList(input: any): Observable<any> {
    return this.restService.request<any, any>({
      method: 'GET',
      url: '/api/saas/editions',
      params: input
    },
    { apiName: this.apiName });
  }

  get(id: string): Observable<EditionDto> {
    return this.restService.request<any, EditionDto>({
      method: 'GET',
      url: `/api/saas/editions/${id}`
    },
    { apiName: this.apiName });
  }

  create(input: CreateEditionDto): Observable<EditionDto> {
    return this.restService.request<any, EditionDto>({
      method: 'POST',
      url: '/api/saas/editions',
      body: input
    },
    { apiName: this.apiName });
  }

  update(id: string, input: UpdateEditionDto): Observable<EditionDto> {
    return this.restService.request<any, EditionDto>({
      method: 'PUT',
      url: `/api/saas/editions/${id}`,
      body: input
    },
    { apiName: this.apiName });
  }

  delete(id: string): Observable<void> {
    return this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/saas/editions/${id}`
    },
    { apiName: this.apiName });
  }
}
