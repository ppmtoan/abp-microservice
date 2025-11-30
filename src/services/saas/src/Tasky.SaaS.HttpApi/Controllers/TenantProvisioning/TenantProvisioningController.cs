using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasky.SaaS.TenantProvisioning;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Tasky.SaaS.Controllers.TenantProvisioning;

[RemoteService(Name = SaaSRemoteServiceConsts.RemoteServiceName)]
[Area(SaaSRemoteServiceConsts.ModuleName)]
[Route("api/saas/tenant-provisioning")]
public class TenantProvisioningController : AbpControllerBase, ITenantProvisioningAppService
{
    private readonly ITenantProvisioningAppService _tenantProvisioningAppService;

    public TenantProvisioningController(ITenantProvisioningAppService tenantProvisioningAppService)
    {
        _tenantProvisioningAppService = tenantProvisioningAppService;
    }

    [HttpPost]
    [Route("provision")]
    [AllowAnonymous] // Can be made public for self-service signup
    public virtual Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input)
    {
        return _tenantProvisioningAppService.ProvisionTenantAsync(input);
    }
}
