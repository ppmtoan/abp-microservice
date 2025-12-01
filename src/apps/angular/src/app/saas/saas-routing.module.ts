import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PermissionGuard } from '@abp/ng.core';

const routes: Routes = [
  {
    path: 'editions',
    loadChildren: () => import('./editions/editions.module').then(m => m.EditionsModule),
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'SaaS.Editions' }
  },
  {
    path: 'subscriptions',
    loadChildren: () => import('./subscriptions/subscriptions.module').then(m => m.SubscriptionsModule),
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'SaaS.Subscriptions' }
  },
  {
    path: 'invoices',
    loadChildren: () => import('./invoices/invoices.module').then(m => m.InvoicesModule),
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'SaaS.Invoices' }
  },
  {
    path: 'tenant-provisioning',
    loadChildren: () => import('./tenant-provisioning/tenant-provisioning.module').then(m => m.TenantProvisioningModule),
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'SaaS.TenantProvisioning.Provision' }
  },
  {
    path: 'host-admin',
    loadChildren: () => import('./host-admin/host-admin.module').then(m => m.HostAdminModule),
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'SaaS.HostAdmin' }
  },
  {
    path: 'tenant-admin',
    loadChildren: () => import('./tenant-admin/tenant-admin.module').then(m => m.TenantAdminModule),
    canActivate: [PermissionGuard],
    data: { requiredPolicy: 'SaaS.TenantAdmin' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaaSRoutingModule {}
