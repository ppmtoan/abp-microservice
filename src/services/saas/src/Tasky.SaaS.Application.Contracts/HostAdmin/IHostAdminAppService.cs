using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Tasky.SaaS.HostAdmin;

public interface IHostAdminAppService : IApplicationService
{
    Task<HostMetricsDto> GetMetricsAsync();
    
    Task<PagedResultDto<TenantDetailDto>> GetTenantsAsync(PagedAndSortedResultRequestDto input);
    
    Task<TenantDetailDto> GetTenantDetailAsync(Guid tenantId);
    
    Task ActivateTenantAsync(Guid tenantId);
    
    Task DeactivateTenantAsync(Guid tenantId);
}
