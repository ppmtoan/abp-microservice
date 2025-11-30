# Input Validation Layer

## Overview

This document describes the input validation layer implemented in the SaaS service. The validation layer provides comprehensive input validation using both data annotations and custom validation attributes to ensure data integrity and security.

## Custom Validation Attributes

### 1. GreaterThanAttribute

Validates that a property value is greater than another property value.

**Location**: `Tasky.SaaS.Application.Contracts/Validation/GreaterThanAttribute.cs`

**Usage**:
```csharp
public class CreateEditionDto
{
    public decimal MonthlyPrice { get; set; }
    
    [GreaterThan(nameof(MonthlyPrice), ErrorMessage = "Yearly price must be greater than monthly price")]
    public decimal YearlyPrice { get; set; }
}
```

**Features**:
- Compares numeric values between properties
- Supports any `IComparable` type
- Custom error messages

### 2. ValidFeatureLimitsAttribute

Validates that feature limits dictionary has valid structure and values.

**Location**: `Tasky.SaaS.Application.Contracts/Validation/ValidFeatureLimitsAttribute.cs`

**Usage**:
```csharp
public class CreateEditionDto
{
    [ValidFeatureLimits(ErrorMessage = "Feature limits must contain required features with positive values")]
    public Dictionary<string, object> FeatureLimits { get; set; }
}
```

**Required Features**:
- `MaxUsers` - Maximum number of users
- `MaxProjects` - Maximum number of projects
- `StorageQuotaGB` - Storage quota in gigabytes
- `APICallsPerMonth` - API calls limit per month

**Features**:
- Validates required feature presence
- Ensures numeric features have positive integer values
- Supports flexible feature dictionary structure

### 3. ValidDateRangeAttribute

Validates that a date is within a valid range relative to another date or current date.

**Location**: `Tasky.SaaS.Application.Contracts/Validation/ValidDateRangeAttribute.cs`

**Usage**:
```csharp
public class CreateSubscriptionDto
{
    [ValidDateRange(MinDaysFromNow = 0, MaxDaysFromNow = 365, ErrorMessage = "Start date must be within the next 365 days")]
    public DateTime? StartDate { get; set; }
}
```

**Properties**:
- `MinDaysFromNow` - Minimum days from current date (default: int.MinValue)
- `MaxDaysFromNow` - Maximum days from current date (default: int.MaxValue)
- `ComparisonProperty` - Property name to compare against
- `MustBeAfter` - Value must be after comparison property
- `MustBeBefore` - Value must be before comparison property

**Features**:
- Validates dates relative to current time
- Compares dates between properties
- Flexible comparison rules

## DTO Validation Rules

### CreateEditionDto

| Property | Validation Rules |
|----------|------------------|
| Name | Required, MaxLength(128) |
| DisplayName | Required, MaxLength(256) |
| Description | MaxLength(1024) |
| MonthlyPrice | Required, Range(0, double.MaxValue) |
| YearlyPrice | Required, Range(0, double.MaxValue), GreaterThan(MonthlyPrice) |
| IsActive | Boolean |
| DisplayOrder | Range(0, int.MaxValue) |
| FeatureLimits | Required, ValidFeatureLimits |

### UpdateEditionDto

| Property | Validation Rules |
|----------|------------------|
| Name | Required, MaxLength(128) |
| DisplayName | Required, MaxLength(256) |
| Description | MaxLength(1024) |
| MonthlyPrice | Required, Range(0, double.MaxValue) |
| YearlyPrice | Required, Range(0, double.MaxValue), GreaterThan(MonthlyPrice) |
| IsActive | Boolean |
| DisplayOrder | Range(0, int.MaxValue) |
| FeatureLimits | Required, ValidFeatureLimits |

### CreateSubscriptionDto

| Property | Validation Rules |
|----------|------------------|
| TenantId | Guid? |
| EditionId | Required, Guid |
| BillingPeriod | Required, Enum |
| StartDate | ValidDateRange(0-365 days) |
| Price | Required, Range(0, double.MaxValue) |
| AutoRenew | Boolean, default: true |
| TrialDays | Range(0, 90) |

### UpdateSubscriptionDto

| Property | Validation Rules |
|----------|------------------|
| BillingPeriod | Required, Enum |
| Price | Required, Range(0, double.MaxValue) |
| AutoRenew | Boolean |

### TenantProvisioningRequestDto

| Property | Validation Rules |
|----------|------------------|
| TenantName | Required, StringLength(64, MinimumLength=3) |
| AdminEmail | Required, EmailAddress, MaxLength(256) |
| AdminPassword | Required, StringLength(128, MinimumLength=6) |
| AdminUserName | StringLength(128, MinimumLength=3) |
| EditionId | Required, Guid |
| BillingPeriod | Required, Enum |
| TrialDays | Range(0, 90) |

### MarkInvoiceAsPaidDto

| Property | Validation Rules |
|----------|------------------|
| PaymentMethod | Required, StringLength(128, MinimumLength=2) |
| PaymentReference | MaxLength(256) |

## Validation Behavior

### Automatic Validation

ABP Framework automatically validates DTOs before they reach application service methods. When validation fails:

1. An `AbpValidationException` is thrown
2. HTTP 400 Bad Request is returned to the client
3. All validation errors are included in the response

### Example Validation Error Response

```json
{
  "error": {
    "code": "400",
    "message": "Validation failed",
    "validationErrors": [
      {
        "message": "Yearly price must be greater than monthly price",
        "members": ["YearlyPrice"]
      },
      {
        "message": "Feature limits must contain required features with positive values",
        "members": ["FeatureLimits"]
      }
    ]
  }
}
```

## Best Practices

### 1. Always Validate Input

- Never trust client input
- Validate at the DTO level before processing
- Use appropriate validation attributes for each field

### 2. Provide Clear Error Messages

- Use custom error messages that clearly explain the validation rule
- Include context about valid values or ranges
- Make error messages user-friendly

### 3. Validate Business Rules

- Use custom attributes for complex business rules
- Keep validation logic in attributes, not in application services
- Make validation reusable across multiple DTOs

### 4. Security Considerations

- Always validate string lengths to prevent buffer overflow
- Validate numeric ranges to prevent integer overflow
- Sanitize email addresses and usernames
- Enforce minimum password length requirements

### 5. Performance

- Validation attributes are evaluated efficiently
- Complex validation should be in custom attributes
- Avoid database calls in validation attributes

## Testing Validation

### Unit Testing Custom Validators

```csharp
[Fact]
public void GreaterThan_Should_Fail_When_Value_Not_Greater()
{
    // Arrange
    var dto = new CreateEditionDto
    {
        MonthlyPrice = 100,
        YearlyPrice = 90  // Invalid: should be > MonthlyPrice
    };
    
    var context = new ValidationContext(dto);
    var results = new List<ValidationResult>();
    
    // Act
    var isValid = Validator.TryValidateObject(dto, context, results, true);
    
    // Assert
    Assert.False(isValid);
    Assert.Contains(results, r => r.MemberNames.Contains("YearlyPrice"));
}
```

### Integration Testing

```csharp
[Fact]
public async Task CreateEdition_Should_Fail_With_Invalid_Pricing()
{
    // Arrange
    var input = new CreateEditionDto
    {
        Name = "Test Edition",
        DisplayName = "Test",
        MonthlyPrice = 100,
        YearlyPrice = 90,  // Invalid
        FeatureLimits = new Dictionary<string, object>
        {
            { "MaxUsers", 10 },
            { "MaxProjects", 5 },
            { "StorageQuotaGB", 100 },
            { "APICallsPerMonth", 10000 }
        }
    };
    
    // Act & Assert
    await Assert.ThrowsAsync<AbpValidationException>(
        async () => await _editionAppService.CreateAsync(input)
    );
}
```

## Extending Validation

### Creating New Custom Validators

1. Create a new class inheriting from `ValidationAttribute`
2. Override the `IsValid` method
3. Implement validation logic
4. Return `ValidationResult.Success` or error message

Example:
```csharp
[AttributeUsage(AttributeTargets.Property)]
public class ValidCurrencyCodeAttribute : ValidationAttribute
{
    private static readonly HashSet<string> ValidCurrencies = new() 
    { 
        "USD", "EUR", "GBP", "JPY" 
    };

    protected override ValidationResult IsValid(
        object value, 
        ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var currency = value.ToString();
        if (!ValidCurrencies.Contains(currency))
        {
            return new ValidationResult(
                $"'{currency}' is not a valid currency code");
        }

        return ValidationResult.Success;
    }
}
```

## References

- [ASP.NET Core Model Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [ABP Validation Documentation](https://docs.abp.io/en/abp/latest/Validation)
- [Data Annotations Reference](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)
