import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { EditionListComponent } from './edition-list/edition-list.component';
import { EditionFormComponent } from './edition-form/edition-form.component';

const routes: Routes = [
  { path: '', component: EditionListComponent }
];

@NgModule({
  declarations: [
    EditionListComponent,
    EditionFormComponent
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
export class EditionsModule {}
