import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { HostDashboardComponent } from './host-dashboard/host-dashboard.component';

const routes: Routes = [
  { path: '', component: HostDashboardComponent }
];

@NgModule({
  declarations: [
    HostDashboardComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    CoreModule,
    ThemeSharedModule,
    NgxDatatableModule,
    NgbDropdownModule,
    RouterModule.forChild(routes)
  ]
})
export class HostAdminModule {}
