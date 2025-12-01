import { RoutesService, eLayoutType } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  { provide: APP_INITIALIZER, useFactory: configureRoutes, deps: [RoutesService], multi: true },
];

function configureRoutes(routesService: RoutesService) {
  return () => {
    routesService.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/saas',
        name: '::Menu:SaaS',
        iconClass: 'fas fa-building',
        order: 2,
        layout: eLayoutType.application,
        requiredPolicy: 'SaaS',
      },
      {
        path: '/saas/editions',
        name: '::Menu:Editions',
        parentName: '::Menu:SaaS',
        iconClass: 'fas fa-layer-group',
        order: 1,
        layout: eLayoutType.application,
        requiredPolicy: 'SaaS.Editions',
      },
      {
        path: '/saas/host-admin',
        name: '::Menu:HostAdmin',
        parentName: '::Menu:SaaS',
        iconClass: 'fas fa-chart-line',
        order: 2,
        layout: eLayoutType.application,
        requiredPolicy: 'SaaS.HostAdmin',
      },
      {
        path: '/saas/tenant-provisioning',
        name: '::Menu:ProvisionTenant',
        parentName: '::Menu:SaaS',
        iconClass: 'fas fa-plus-circle',
        order: 3,
        layout: eLayoutType.application,
        requiredPolicy: 'SaaS.TenantProvisioning.Provision',
      },
      {
        path: '/saas/tenant-admin',
        name: '::Menu:TenantDashboard',
        parentName: '::Menu:SaaS',
        iconClass: 'fas fa-tachometer-alt',
        order: 4,
        layout: eLayoutType.application,
        requiredPolicy: 'SaaS.TenantAdmin',
      },
    ]);
  };
}
