import { Component, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { EditionDto } from '../../shared/models';
import { EditionService } from '../../shared/services';

@Component({
  selector: 'app-edition-list',
  templateUrl: './edition-list.component.html',
  providers: [ListService]
})
export class EditionListComponent implements OnInit {
  editions = { items: [], totalCount: 0 } as PagedResultDto<EditionDto>;
  isModalOpen = false;
  selectedEdition: EditionDto | null = null;

  constructor(
    public readonly list: ListService,
    private editionService: EditionService,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit() {
    const editionStreamCreator = (query:any) => this.editionService.getList(query);
    this.list.hookToQuery(editionStreamCreator).subscribe((response) => {
      this.editions = response as PagedResultDto<EditionDto>;
    });
  }

  createEdition() {
    this.selectedEdition = null;
    this.isModalOpen = true;
  }

  editEdition(id: string) {
    this.editionService.get(id).subscribe((edition) => {
      this.selectedEdition = edition;
      this.isModalOpen = true;
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSure', '::AreYouSureToDelete').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.editionService.delete(id).subscribe(() => this.list.get());
      }
    });
  }
}
