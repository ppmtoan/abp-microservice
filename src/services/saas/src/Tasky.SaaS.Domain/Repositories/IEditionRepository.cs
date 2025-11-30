using System;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Volo.Abp.Domain.Repositories;

namespace Tasky.SaaS.Repositories;

public interface IEditionRepository : IRepository<Edition, Guid>
{
}
