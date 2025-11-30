using System;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Volo.Abp.Domain.Repositories;

namespace Tasky.SaaS.Repositories;

public interface IInvoiceRepository : IRepository<Invoice, Guid>
{
}
