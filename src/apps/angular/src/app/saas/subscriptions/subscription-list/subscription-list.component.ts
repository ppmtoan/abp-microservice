import { Component, OnInit } from '@angular/core';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { SubscriptionDto } from '../../shared/models';
import { SubscriptionService } from '../../shared/services';

@Component({
  selector: 'app-subscription-list',
  templateUrl: './subscription-list.component.html',
  providers: [ListService]
})
export class SubscriptionListComponent implements OnInit {
  subscriptions = { items: [], totalCount: 0 } as PagedResultDto<SubscriptionDto>;

  constructor(
    public readonly list: ListService,
    private subscriptionService: SubscriptionService,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit() {
    const subscriptionStreamCreator = (query: any) => this.subscriptionService.getList(query);
    this.list.hookToQuery(subscriptionStreamCreator).subscribe((response) => {
      this.subscriptions = response as PagedResultDto<SubscriptionDto>;
    });
  }

  renewSubscription(id: string) {
    this.confirmation.warn('::RenewSubscription', '::AreYouSureToRenewSubscription').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.subscriptionService.renew(id).subscribe(() => {
          this.list.get();
        });
      }
    });
  }

  cancelSubscription(id: string) {
    this.confirmation.warn('::CancelSubscription', '::AreYouSureToCancelSubscription').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.subscriptionService.cancel(id).subscribe(() => {
          this.list.get();
        });
      }
    });
  }

  getStatusClass(status: number): string {
    switch (status) {
      case 0: return 'bg-success'; // Active
      case 1: return 'bg-secondary'; // Suspended
      case 2: return 'bg-danger'; // Cancelled
      case 3: return 'bg-danger'; // PastDue
      case 4: return 'bg-warning'; // Trial
      default: return 'bg-secondary';
    }
  }

  getStatusText(status: number): string {
    const statuses = ['Active', 'Suspended', 'Cancelled', 'PastDue', 'Trial'];
    return statuses[status] || 'Unknown';
  }

  getBillingPeriodText(period: number): string {
    const periods = ['Monthly', 'Yearly', 'Quarterly'];
    return periods[period] || 'Unknown';
  }
}
