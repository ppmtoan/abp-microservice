using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tasky.SaaS.TenantAdmin;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Tasky.SaaS.Controllers.TenantAdmin;

[RemoteService(Name = SaaSRemoteServiceConsts.RemoteServiceName)]
[Area(SaaSRemoteServiceConsts.ModuleName)]
[Route("api/saas/tenant-admin")]
public class TenantAdminController : AbpControllerBase, ITenantAdminAppService
{
    private readonly ITenantAdminAppService _tenantAdminAppService;

    public TenantAdminController(ITenantAdminAppService tenantAdminAppService)
    {
        _tenantAdminAppService = tenantAdminAppService;
    }

    [HttpGet]
    [Route("dashboard")]
    public virtual Task<TenantDashboardDto> GetDashboardAsync()
    {
        return _tenantAdminAppService.GetDashboardAsync();
    }
}
