using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Tasky.SaaS.TenantProvisioning;

public interface ITenantProvisioningAppService : IApplicationService
{
    Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input);
}
