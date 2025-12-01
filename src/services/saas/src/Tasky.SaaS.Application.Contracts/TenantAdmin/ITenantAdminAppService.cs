using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Tasky.SaaS.TenantAdmin;

public interface ITenantAdminAppService : IApplicationService
{
    Task<TenantDashboardDto> GetDashboardAsync();
}
