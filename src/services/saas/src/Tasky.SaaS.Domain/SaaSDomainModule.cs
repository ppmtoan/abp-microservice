using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace Tasky.SaaS;

[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(SaaSDomainSharedModule))]
[DependsOn(typeof(AbpTenantManagementDomainModule))]
[DependsOn(typeof(AbpIdentityDomainModule))]
public class SaaSDomainModule : AbpModule { }
