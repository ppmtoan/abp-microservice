import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { TenantProvisioningComponent } from './tenant-provisioning.component';

const routes: Routes = [
  { path: '', component: TenantProvisioningComponent }
];

@NgModule({
  declarations: [
    TenantProvisioningComponent
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
export class TenantProvisioningModule {}
