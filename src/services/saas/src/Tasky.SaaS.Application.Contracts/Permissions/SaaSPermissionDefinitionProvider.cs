using Tasky.SaaS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Tasky.SaaS.Permissions;

public class SaaSPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var saasGroup = context.AddGroup(SaaSPermissions.GroupName, L("Permission:SaaS"));

        // Tenants
        var tenantsPermission = saasGroup.AddPermission(
            SaaSPermissions.Tenants.Default,
            L("Permission:SaaS:Tenants")
        );
        tenantsPermission.AddChild(
            SaaSPermissions.Tenants.Create,
            L("Permission:SaaS:Tenants.Create")
        );
        tenantsPermission.AddChild(
            SaaSPermissions.Tenants.Update,
            L("Permission:SaaS:Tenants.Update")
        );
        tenantsPermission.AddChild(
            SaaSPermissions.Tenants.Delete,
            L("Permission:SaaS:Tenants.Delete")
        );
        tenantsPermission.AddChild(
            SaaSPermissions.Tenants.ManageFeatures,
            L("Permission:SaaS:Tenants.ManageFeatures")
        );
        tenantsPermission.AddChild(
            SaaSPermissions.Tenants.ManageConnectionStrings,
            L("Permission:SaaS:Tenants.ManageConnectionStrings")
        );

        // Editions
        var editionsPermission = saasGroup.AddPermission(
            SaaSPermissions.Editions.Default,
            L("Permission:SaaS:Editions")
        );
        editionsPermission.AddChild(
            SaaSPermissions.Editions.Create,
            L("Permission:SaaS:Editions.Create")
        );
        editionsPermission.AddChild(
            SaaSPermissions.Editions.Update,
            L("Permission:SaaS:Editions.Update")
        );
        editionsPermission.AddChild(
            SaaSPermissions.Editions.Delete,
            L("Permission:SaaS:Editions.Delete")
        );

        // Subscriptions
        var subscriptionsPermission = saasGroup.AddPermission(
            SaaSPermissions.Subscriptions.Default,
            L("Permission:SaaS:Subscriptions")
        );
        subscriptionsPermission.AddChild(
            SaaSPermissions.Subscriptions.Create,
            L("Permission:SaaS:Subscriptions.Create")
        );
        subscriptionsPermission.AddChild(
            SaaSPermissions.Subscriptions.Update,
            L("Permission:SaaS:Subscriptions.Update")
        );
        subscriptionsPermission.AddChild(
            SaaSPermissions.Subscriptions.Delete,
            L("Permission:SaaS:Subscriptions.Delete")
        );
        subscriptionsPermission.AddChild(
            SaaSPermissions.Subscriptions.Manage,
            L("Permission:SaaS:Subscriptions.Manage")
        );

        // Invoices
        var invoicesPermission = saasGroup.AddPermission(
            SaaSPermissions.Invoices.Default,
            L("Permission:SaaS:Invoices")
        );
        invoicesPermission.AddChild(
            SaaSPermissions.Invoices.MarkAsPaid,
            L("Permission:SaaS:Invoices.MarkAsPaid")
        );
        invoicesPermission.AddChild(
            SaaSPermissions.Invoices.Cancel,
            L("Permission:SaaS:Invoices.Cancel")
        );

        // Tenant Provisioning (public)
        saasGroup.AddPermission(
            SaaSPermissions.TenantProvisioning.Default,
            L("Permission:SaaS:TenantProvisioning")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SaaSResource>(name);
    }
}
