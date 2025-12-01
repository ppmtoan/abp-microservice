import { BillingPeriod } from './subscription.model';

export interface TenantProvisioningRequestDto {
  tenantName: string;
  adminEmail: string;
  adminPassword: string;
  editionId: string;
  billingPeriod: BillingPeriod;
}

export interface TenantProvisioningResultDto {
  tenantId: string;
  tenantName: string;
  adminEmail: string;
  subscriptionId: string;
  initialInvoiceId: string;
}

export interface HostMetricsDto {
  totalTenants: number;
  activeTenants: number;
  activeSubscriptions: number;
  totalMRR: number;
  totalARR: number;
  newTenantsThisMonth: number;
  churnRate: number;
}

export interface TenantDetailDto {
  id: string;
  name: string;
  isActive: boolean;
  creationTime: string;
  editionName?: string;
  subscriptionStatus?: string;
  subscriptionEndDate?: string;
  totalRevenue: number;
  connectionStrings: string[];
}

export interface TenantDashboardDto {
  editionName: string;
  subscriptionStatus: string;
  subscriptionEndDate: string;
  nextBillingDate: string;
  featureLimits: Record<string, any>;
  pendingInvoices: any[];
  currentUserCount: number;
}
