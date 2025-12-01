# Contributing to Tasky

Thank you for your interest in contributing to Tasky! This document provides guidelines and instructions for contributing to this project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Testing](#testing)
- [Documentation](#documentation)

## Code of Conduct

This project adheres to a code of conduct that all contributors are expected to follow. Please be respectful and professional in all interactions.

## Getting Started

### Prerequisites

Ensure you have the following installed:
- .NET 9.0 SDK
- Docker Desktop
- Git
- Your favorite IDE (VS Code, Visual Studio, Rider)

### Initial Setup

1. **Fork the repository** on GitHub

2. **Clone your fork**:
   ```bash
   git clone https://github.com/YOUR_USERNAME/tasky.git
   cd tasky
   ```

3. **Add upstream remote**:
   ```bash
   git remote add upstream https://github.com/ORIGINAL_OWNER/tasky.git
   ```

4. **Run setup script**:
   ```bash
   ./scripts/dev-setup.sh
   ```

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
# or
git checkout -b fix/bug-description
```

Branch naming conventions:
- `feature/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Test additions or modifications

### 2. Make Your Changes

Follow the [Coding Standards](#coding-standards) when writing code.

### 3. Test Your Changes

```bash
# Run unit tests
make test

# Run specific service tests
dotnet test src/services/saas/test/Tasky.SaaS.Domain.Tests

# Check code formatting
make format
```

### 4. Commit Your Changes

Follow [Commit Guidelines](#commit-guidelines) for commit messages.

```bash
git add .
git commit -m "feat: add new feature description"
```

### 5. Keep Your Branch Updated

```bash
git fetch upstream
git rebase upstream/main
```

### 6. Push to Your Fork

```bash
git push origin feature/your-feature-name
```

### 7. Create Pull Request

Open a pull request from your fork to the main repository.

## Coding Standards

### C# Coding Style

This project follows standard C# coding conventions enforced by `.editorconfig`:

- **Indentation**: 4 spaces
- **Line endings**: LF (Unix-style)
- **Naming conventions**:
  - PascalCase for classes, methods, properties
  - camelCase for local variables, parameters
  - _camelCase for private fields (with underscore prefix)
  - IPascalCase for interfaces (with I prefix)
- **Braces**: Opening brace on new line
- **`var` usage**: Use for obvious types, avoid for built-in types

### File Organization

```csharp
// 1. Using directives (sorted)
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

// 2. Namespace
namespace Tasky.SaaS.Domain.Subscriptions;

// 3. Class/Interface definition
public class Subscription : AggregateRoot<Guid>
{
    // 4. Constants
    public const int MaxNameLength = 128;

    // 5. Private fields
    private readonly List<SubscriptionItem> _items;

    // 6. Constructors
    public Subscription(Guid id) : base(id)
    {
        _items = new List<SubscriptionItem>();
    }

    // 7. Properties
    public string Name { get; private set; }

    // 8. Methods (public first, then protected, then private)
    public void AddItem(SubscriptionItem item)
    {
        // Implementation
    }

    private void ValidateName(string name)
    {
        // Implementation
    }
}
```

### Domain-Driven Design Patterns

Follow DDD principles:

1. **Aggregates**: Keep aggregates small and focused
2. **Value Objects**: Use for concepts without identity
3. **Domain Services**: For operations spanning multiple aggregates
4. **Domain Events**: For cross-aggregate consistency
5. **Specifications**: For reusable query logic

### Example: Creating a Value Object

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { } // For EF Core

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required");

        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

## Commit Guidelines

Use [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, missing semicolons, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks
- `perf`: Performance improvements
- `ci`: CI/CD changes

### Examples

```bash
feat(saas): add subscription renewal logic

Implement automatic subscription renewal when subscription expires.
Includes domain event for renewal notification.

Closes #123

---

fix(identity): resolve user registration validation issue

The email validation was not properly checking for duplicate emails.

Fixes #456

---

docs: update README with Docker setup instructions

---

refactor(projects): extract project creation logic to domain service
```

## Pull Request Process

### Before Submitting

1. ✅ Code follows coding standards
2. ✅ All tests pass
3. ✅ Code is properly formatted (`make format`)
4. ✅ Documentation is updated if needed
5. ✅ Commit messages follow conventions
6. ✅ Branch is up to date with main

### PR Title Format

Use the same format as commit messages:

```
feat(saas): add subscription renewal feature
```

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
Describe testing performed

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests added/updated
- [ ] All tests pass locally
```

### Review Process

1. At least one maintainer must approve
2. All CI checks must pass
3. All review comments must be resolved
4. Branch must be up to date with main

## Testing

### Unit Tests

Write unit tests for:
- Domain logic
- Application services
- Validators
- Value objects

```csharp
public class MoneyTests
{
    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(-10, "USD"));
    }

    [Theory]
    [InlineData(100, "USD", 100, "USD", true)]
    [InlineData(100, "USD", 100, "EUR", false)]
    public void Equals_ComparesCorrectly(
        decimal amount1, string currency1,
        decimal amount2, string currency2,
        bool expected)
    {
        // Arrange
        var money1 = new Money(amount1, currency1);
        var money2 = new Money(amount2, currency2);

        // Act
        var result = money1.Equals(money2);

        // Assert
        Assert.Equal(expected, result);
    }
}
```

### Integration Tests

Test interactions between components:
- Repository operations
- Application service workflows
- API endpoints

### Running Tests

```bash
# All tests
make test

# Specific project
dotnet test src/services/saas/test/Tasky.SaaS.Domain.Tests

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Documentation

### Code Documentation

Use XML documentation comments for public APIs:

```csharp
/// <summary>
/// Creates a new subscription for the specified tenant.
/// </summary>
/// <param name="tenantId">The tenant identifier.</param>
/// <param name="editionId">The edition identifier.</param>
/// <param name="startDate">The subscription start date.</param>
/// <returns>The created subscription.</returns>
/// <exception cref="ArgumentException">
/// Thrown when tenantId or editionId is empty.
/// </exception>
public Task<Subscription> CreateSubscriptionAsync(
    Guid tenantId,
    Guid editionId,
    DateTime startDate)
{
    // Implementation
}
```

### Architecture Documentation

When adding new features, update:
- README.md - High-level overview
- Architecture diagrams (if applicable)
- API documentation (Swagger/OpenAPI)

### Examples

Provide examples for complex features:

```csharp
// Example: Creating a subscription with custom features
var subscription = await _subscriptionManager.CreateSubscriptionAsync(
    tenantId: Guid.NewGuid(),
    editionId: standardEditionId,
    startDate: DateTime.UtcNow,
    durationMonths: 12,
    customFeatures: new Dictionary<string, object>
    {
        ["MaxProjects"] = 50,
        ["StorageQuotaGB"] = 100
    }
);
```

## Questions?

- Open an issue for bugs or feature requests
- Start a discussion for questions or ideas
- Check existing issues and discussions first

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to Tasky! 🎉
