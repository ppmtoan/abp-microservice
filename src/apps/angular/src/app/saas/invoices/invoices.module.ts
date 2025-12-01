import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { InvoiceListComponent } from './invoice-list/invoice-list.component';

const routes: Routes = [
  { path: '', component: InvoiceListComponent }
];

@NgModule({
  declarations: [InvoiceListComponent],
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    NgxDatatableModule,
    NgbDropdownModule,
    RouterModule.forChild(routes)
  ]
})
export class InvoicesModule {}
