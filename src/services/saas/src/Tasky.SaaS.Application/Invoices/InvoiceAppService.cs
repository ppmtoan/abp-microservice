using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tasky.SaaS.DomainServices;
using Tasky.SaaS.Aggregates.EditionAggregate;
using Tasky.SaaS.Aggregates.SubscriptionAggregate;
using Tasky.SaaS.Aggregates.BillingAggregate;
using Tasky.SaaS.Enums;
using Tasky.SaaS.Permissions;
using Tasky.SaaS.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Tasky.SaaS.Invoices;

public class InvoiceAppService : ApplicationService, IInvoiceAppService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly InvoiceManager _invoiceManager;

    public InvoiceAppService(
        IInvoiceRepository invoiceRepository,
        ISubscriptionRepository subscriptionRepository,
        InvoiceManager invoiceManager)
    {
        _invoiceRepository = invoiceRepository;
        _subscriptionRepository = subscriptionRepository;
        _invoiceManager = invoiceManager;
    }

    public async Task<PagedResultDto<InvoiceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        await CheckPolicyAsync(SaaSPermissions.Invoices.Default);

        var query = await _invoiceRepository.GetQueryableAsync();

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query
            .OrderByDescending(i => i.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var invoices = await AsyncExecuter.ToListAsync(query);
        var dtos = ObjectMapper.Map<List<Invoice>, List<InvoiceDto>>(invoices);

        return new PagedResultDto<InvoiceDto>(totalCount, dtos);
    }

    public async Task<InvoiceDto> GetAsync(Guid id)
    {
        await CheckPolicyAsync(SaaSPermissions.Invoices.Default);

        var invoice = await _invoiceRepository.GetAsync(id);

        if (invoice == null)
        {
            throw new BusinessException(SaaSErrorCodes.InvoiceNotFound)
                .WithData("InvoiceId", id);
        }

        return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
    }

    public async Task<List<InvoiceDto>> GetCurrentTenantInvoicesAsync()
    {
        if (!CurrentTenant.IsAvailable)
        {
            throw new BusinessException(SaaSErrorCodes.TenantNotAvailable);
        }

        var query = await _invoiceRepository.GetQueryableAsync();
        var invoices = await AsyncExecuter.ToListAsync(
            query.Where(i => i.TenantId == CurrentTenant.Id)
                .OrderByDescending(i => i.CreationTime)
        );

        return ObjectMapper.Map<List<Invoice>, List<InvoiceDto>>(invoices);
    }

    public async Task<InvoiceDto> MarkAsPaidAsync(Guid id, MarkInvoiceAsPaidDto input)
    {
        await CheckPolicyAsync(SaaSPermissions.Invoices.MarkAsPaid);

        var invoice = await _invoiceRepository.GetAsync(id);
        
        // Delegate business logic to domain service
        _invoiceManager.ProcessPayment(invoice, input.PaymentMethod, input.PaymentReference);

        await _invoiceRepository.UpdateAsync(invoice);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await GetAsync(id);
    }

    public async Task CancelAsync(Guid id)
    {
        await CheckPolicyAsync(SaaSPermissions.Invoices.Cancel);

        var invoice = await _invoiceRepository.GetAsync(id);
        
        // Validate cancellation through domain service
        if (!_invoiceManager.CanCancelInvoice(invoice))
        {
            throw new BusinessException(SaaSErrorCodes.CannotPayInvoice)
                .WithData("InvoiceId", id)
                .WithData("Status", invoice.Status);
        }
        
        invoice.Cancel();

        await _invoiceRepository.UpdateAsync(invoice);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task ProcessOverdueInvoicesAsync()
    {
        await CheckPolicyAsync(SaaSPermissions.Invoices.Default);

        // Delegate business logic to domain service
        var processedCount = await _invoiceManager.ProcessOverdueInvoicesAsync();
        
        await CurrentUnitOfWork.SaveChangesAsync();

        Logger.LogInformation($"Processed {processedCount} overdue invoices");
    }
}
