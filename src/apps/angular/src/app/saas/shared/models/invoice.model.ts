export enum InvoiceStatus {
  Pending = 0,
  Paid = 1,
  Overdue = 2,
  Cancelled = 3
}

export interface InvoiceDto {
  id: string;
  tenantId: string;
  tenantName?: string;
  subscriptionId: string;
  invoiceNumber: string;
  amount: number;
  dueDate: string;
  status: InvoiceStatus;
  paidDate?: string;
  creationTime: string;
}

export interface MarkInvoiceAsPaidDto {
  paymentMethod: string;
  transactionId: string;
}
