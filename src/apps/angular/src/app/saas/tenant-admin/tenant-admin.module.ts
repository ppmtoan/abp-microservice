import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { TenantDashboardComponent } from './tenant-dashboard/tenant-dashboard.component';

const routes: Routes = [
  { path: '', component: TenantDashboardComponent }
];

@NgModule({
  declarations: [
    TenantDashboardComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    CoreModule,
    ThemeSharedModule,
    RouterModule.forChild(routes)
  ]
})
export class TenantAdminModule {}
