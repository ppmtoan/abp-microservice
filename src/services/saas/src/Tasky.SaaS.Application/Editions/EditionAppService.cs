using System;
using System.Text.Json;
using Tasky.SaaS.Entities;
using Tasky.SaaS.Permissions;
using Tasky.SaaS.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Tasky.SaaS.Editions;

public class EditionAppService : CrudAppService<Edition, EditionDto, Guid, PagedAndSortedResultRequestDto, CreateEditionDto, UpdateEditionDto>, IEditionAppService
{
    public EditionAppService(IEditionRepository repository) : base(repository)
    {
        GetPolicyName = SaaSPermissions.Editions.Default;
        GetListPolicyName = SaaSPermissions.Editions.Default;
        CreatePolicyName = SaaSPermissions.Editions.Create;
        UpdatePolicyName = SaaSPermissions.Editions.Update;
        DeletePolicyName = SaaSPermissions.Editions.Delete;
    }
}
