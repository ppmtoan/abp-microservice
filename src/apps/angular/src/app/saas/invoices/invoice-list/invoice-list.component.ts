import { Component, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { InvoiceDto, InvoiceStatus } from '../../shared/models/invoice.model';
import { InvoiceService } from '../../shared/services/invoice.service';

@Component({
  selector: 'app-invoice-list',
  templateUrl: './invoice-list.component.html',
  providers: [ListService]
})
export class InvoiceListComponent implements OnInit {
  invoices = { items: [], totalCount: 0 } as PagedResultDto<InvoiceDto>;

  constructor(
    public readonly list: ListService,
    private invoiceService: InvoiceService,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit() {
    const invoiceStreamCreator = (query) => this.invoiceService.getList(query);

    this.list.hookToQuery(invoiceStreamCreator).subscribe((response) => {
      this.invoices = response as PagedResultDto<InvoiceDto>;
    });
  }

  markAsPaid(id: string) {
    this.confirmation.warn('::AreYouSure', '::MarkInvoiceAsPaidConfirmationMessage').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        const input = { paymentMethod: 'Manual', transactionId: '' };
        this.invoiceService.markAsPaid(id, input).subscribe(() => {
          this.list.get();
        });
      }
    });
  }

  getStatusText(status: InvoiceStatus): string {
    switch (status) {
      case InvoiceStatus.Pending:
        return 'Pending';
      case InvoiceStatus.Paid:
        return 'Paid';
      case InvoiceStatus.Overdue:
        return 'Overdue';
      case InvoiceStatus.Cancelled:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }

  getStatusClass(status: InvoiceStatus): string {
    switch (status) {
      case InvoiceStatus.Pending:
        return 'badge-warning';
      case InvoiceStatus.Paid:
        return 'badge-success';
      case InvoiceStatus.Overdue:
        return 'badge-danger';
      case InvoiceStatus.Cancelled:
        return 'badge-secondary';
      default:
        return 'badge-light';
    }
  }
}
