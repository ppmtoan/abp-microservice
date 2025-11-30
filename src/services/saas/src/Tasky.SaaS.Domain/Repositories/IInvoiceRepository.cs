using System;
using Tasky.SaaS.Entities;
using Volo.Abp.Domain.Repositories;

namespace Tasky.SaaS.Repositories;

public interface IInvoiceRepository : IRepository<Invoice, Guid>
{
}
