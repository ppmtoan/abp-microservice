export enum BillingPeriod {
  Monthly = 0,
  Yearly = 1,
  Quarterly = 2
}

export enum SubscriptionStatus {
  Active = 0,
  Suspended = 1,
  Cancelled = 2,
  PastDue = 3,
  Trial = 4
}

export interface SubscriptionDto {
  id: string;
  tenantId: string;
  tenantName?: string;
  editionId: string;
  editionName?: string;
  startDate: string;
  endDate: string;
  billingPeriod: BillingPeriod;
  nextBillingDate: string;
  status: SubscriptionStatus;
  creationTime: string;
}

export interface CreateSubscriptionDto {
  tenantId: string;
  editionId: string;
  billingPeriod: BillingPeriod;
}

export interface UpdateSubscriptionDto {
  billingPeriod: BillingPeriod;
}
