# `Microlens.Synthesizer.Core`

Generate compilable `Bogus` `Faker<T>` classes directly from `C#` classes and records, using the `Roslyn` semantic model — no runtime type inspection, no hand-written boilerplate.

**`Microlens.Synthesizer.Core`** is a `Roslyn`-based `Faker<T>` generation engine for .NET that **analyzes**, **maps** and **emits** `Bogus` fakers directly from a `SemanticModel`.

Unlike reflection-based fake-data generators that build values at runtime, **`Microlens.Synthesizer.Core`** emits an actual `Faker<T>` class as `C#` source — reviewable, debuggable, and just as fast as a hand-written one, because it is one.

[What's New](#whats-new) | [Why `Microlens.Synthesizer.Core`?](#why-microlenssynthesizercore) | [Visual Studio Extension](#visual-studio-extension) | [Requirements](#requirements) | [Quick Start](#quick-start) | [Quick Example](#quick-example) | [Features](#features) | [Extensible Architecture](#extensible-architecture) | [Comparison](#comparison) | [Limitations](#limitations) | [When Not To Use `Microlens.Synthesizer.Core`](#when-not-to-use-microlenssynthesizercore) | [License](#license)

---

## What's New

* Inherited properties: Base-class properties are now discovered and emitted base-first. A hiding or overriding property replaces its base.
* Generic source types: `Box<T>` now produces `BoxFaker<T> : Faker<Box<T>>`, with type parameter constraints carried over.
* Accessibility-aware generation: The Faker mirrors the source type's accessibility (`public` / `internal`). Only properties whose getter is accessible from the source assembly are emitted.
* Syntax validation: Every generated Faker is parsed before it is returned. A syntax error, for example from a custom provider expression, throws `ApplicationException` instead of producing a broken file.
* Provider helpers: `ITypeSymbol.ToDisplayName()` and `ITypeSymbol.GetReferencedNamespaces()` are now public for custom providers.
* Changed: Convention-based `string` mapping moved into the built-in `string` provider. A custom `string` provider registered before `AddCoreServices()` now replaces conventions entirely.
* Changed: `DateTimeProvider`, `DateTimeOffsetProvider` and `GuidProvider` no longer implement `INamespaceProvider`. A `using` directive is emitted only when the generated code names a type from that namespace.
* Fixed: Types in the global namespace produced an invalid `namespace` block.
* Fixed: `internal` source types produced `public` Fakers, causing compiler error CS0060.
* Fixed: `private`, `protected` and write-only properties produced non-compilable rules.
* Fixed: Nested enum property types were emitted without their containing type.
* Fixed: Static classes, and classes nested inside structs, could be selected as the target type.
* Fixed: Keyword identifiers (e.g. `event`) were emitted unescaped.
* Fixed: `Guid` and `DateTimeOffset` detection matched any namespace named `System`, case-insensitively.
* Fixed: A redundant `using System;` was emitted for scalar `DateTime`, `DateTimeOffset` and `Guid` properties.

---

## Why `Microlens.Synthesizer.Core`?

Most `Faker<T>` classes are written and maintained by hand, one `RuleFor` at a time, and quietly drift out of sync the moment the source class changes.

Examples of typical use cases:

* **Test Data Setup**: Generate realistic seed data for unit and integration tests without hand-authoring every property.
* **Contract Prototyping**: Produce sample payloads for a new `DTO` the moment it's defined.
* **Consistent Test Fixtures**: Keep Fakers in sync with their source class by regenerating instead of manually patching them.
* **Onboarding New Types**: Every new entity or `DTO` gets a matching Faker with zero extra effort.

---

## Visual Studio Extension

Most users won't need this package directly — the companion Visual Studio extension generates and saves `Faker<T>` files for you, right from Solution Explorer.

**Generate Bogus Fakers** extension is available for Visual Studio [2019](https://marketplace.visualstudio.com/items?itemName=microlens.legacysynthesizer), and [2022 / 2026](https://marketplace.visualstudio.com/items?itemName=microlens.modernsynthesizer).

Use `Microlens.Synthesizer.Core` directly only when you need the same generation logic outside Visual Studio — a build step, a source generator, or your own tooling.

---

## Requirements

* Target framework: `netstandard2.0`
* `Microsoft.CodeAnalysis.CSharp` `4.14.0` or later
* `Microsoft.Extensions.DependencyInjection` `9.0.20` or later
* The project that compiles the generated Fakers must reference [`Bogus`](https://www.nuget.org/packages/Bogus).

When used inside a source generator:

* The host compiler must be `Roslyn 4.14` or later (Visual Studio 17.14 / .NET SDK 9.0.300 or later).
* Analyzer packages do not flow NuGet dependencies, so `Microlens.Synthesizer.Core` and `Microsoft.Extensions.DependencyInjection` must be packed alongside the generator.

---

## Quick Start

### Installation

```bash
dotnet add package Microlens.Synthesizer.Core
```

### Implement `IOptions`

`Microlens.Synthesizer.Core` ships no default `IOptions` implementation. Default values are exposed as constants on `Registry`.

```csharp
using Microlens.Synthesizer.Core.Options;
using Microlens.Synthesizer.Core.Shared;

public sealed class GeneratorOptions : IOptions {
    public bool OverwriteExisting => Registry.OptionsOverwriteExistingDefaultValue;

    public int ElementCount => Registry.OptionsElementCountDefaultValue;

    public string FakerSuffix => Registry.OptionsFakerSuffixDefaultValue;

    public string LogFilePath => Registry.OptionsLogFilePathDefaultValue;

    public Registry.LogWritingMode LogWritingMode => Registry.OptionsLogWritingModeDefaultValue;
}
```

| Member | Consumed By |
| :--- | :--- |
| `ElementCount` | Array, `List<T>`, `HashSet<T>` and `Dictionary<TKey, TValue>` rules |
| `FakerSuffix` | Generated class name (`IBogusGenerator`) and output file name (`IFileWriter`) |
| `OverwriteExisting` | `IFileWriter` only |
| `LogFilePath`, `LogWritingMode` | Not read by `Microlens.Synthesizer.Core`; available to the host for its own logging |

### Register Services

```csharp
using Microlens.Synthesizer.Core.Extensions;
using Microlens.Synthesizer.Core.Options;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCoreServices();
services.AddSingleton<IOptions, GeneratorOptions>();

using var provider = services.BuildServiceProvider();
```

### Analyze And Generate

The `SemanticModel` must come from a compilation with metadata references. Unresolved types become error types and are emitted as `// TODO:`.

```csharp
using System;
using System.IO;
using Microlens.Synthesizer.Core.Analyzers;
using Microlens.Synthesizer.Core.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

string[] paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
var references = new MetadataReference[paths.Length];

for (int i = 0; i < paths.Length; i++) {
    references[i] = MetadataReference.CreateFromFile(paths[i]);
}

SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText("Customer.cs"));
CSharpCompilation compilation = CSharpCompilation.Create("Scratch", [tree], references);
SemanticModel model = compilation.GetSemanticModel(tree);

var metadata = provider.GetRequiredService<IClassAnalyzer>().Analyze(model);
string fakerSource = provider.GetRequiredService<IBogusGenerator>().Generate(metadata);
```

`fakerSource` is a complete `Faker<T>` class as a string. Inside Visual Studio or a source generator, use the `SemanticModel` of the existing compilation instead.

### Persist (Optional)

```csharp
using Microlens.Synthesizer.Core.Persistence;

var writer = provider.GetRequiredService<IFileWriter>();
bool written = writer.TryWrite("Customer.cs", outputDirectory, fakerSource, out string outputPath);
```

* The output file is `{source file name}{FakerSuffix}.cs` in `outputDirectory`.
* `TryWrite` returns `false` when the file exists and `OverwriteExisting` is `false`.
* Any other I/O failure throws `ApplicationException`.

---

## Quick Example

Given a class:

```csharp
using System.Collections.Generic;

namespace YourNamespace {
    public class Customer {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public List<string> Tags { get; set; }
    }
}
```

**`Microlens.Synthesizer.Core`** analyzes it through `Roslyn` and emits (`ElementCount = 3`):

```csharp
using Bogus;
using System.Collections.Generic;

namespace YourNamespace
{
    public class CustomerFaker : Faker<Customer>
    {
        public CustomerFaker()
        {
            RuleFor(x => x.FirstName, f => f.Name.FirstName());
            RuleFor(x => x.Email, f => f.Internet.Email());
            RuleFor(x => x.Age, f => f.Random.Int());
            RuleFor(x => x.Tags, f => new List<string>(f.Make(3, () => f.Name.FullName())));
        }
    }
}
```

* No manually written `RuleFor` calls.
* No stale Fakers after a property is renamed — just regenerate.

---

## Features

### Semantic-Model-Based Analysis

Type and property discovery runs against a `Roslyn` `SemanticModel`:

* **Target**: The first non-static, top-level `class` or `record class` in the file. `partial` declarations across files are merged.
* **Properties**:
  * Instance, non-indexer properties with a getter and a `set` or `init` accessor.
  * The getter must be accessible from the source assembly (`public`, `internal`, `protected internal`).
  * Inherited properties are included, emitted base-first; a hiding or overriding property replaces its base.
* **Nullable value types**: `int?`, `DateTime?`, `Status?` resolve as their underlying type.

### Faithful Output Shape

* **Accessibility**: The Faker mirrors the source type — `public` or `internal`.
* **Generic types**: Type parameters and constraints are carried over:

```csharp
  public class BoxFaker<T> : Faker<Box<T>> where T : class
```

* **Global namespace**: Types without a namespace produce a Faker without a namespace block.
* **Keywords**: Keyword identifiers are escaped (`x => x.@event`).
* **Nested types**: Nested enums and other nested types are emitted with their containing type (`Order.Status`).

### Supported Types

| Type | Emitted |
| :--- | :--- |
| `bool` | `f.Random.Bool()` |
| `char` | `f.Random.Char()` |
| `byte` | `f.Random.Byte()` |
| `short` | `f.Random.Short()` |
| `int` | `f.Random.Int()` |
| `long` | `f.Random.Long()` |
| `float` | `f.Random.Float()` |
| `double` | `f.Random.Double()` |
| `decimal` | `f.Finance.Amount()` |
| `string` | Convention factory, else `f.Name.FullName()` |
| `DateTime` | `f.Date.Past()` |
| `DateTimeOffset` | `f.Date.PastOffset()` |
| `Guid` | `f.Random.Guid()` |
| `enum` | `f.PickRandom<TEnum>()` |
| `T[]`, `List<T>`, `HashSet<T>` | `f.Make(ElementCount, …)` |
| `Dictionary<TKey, TValue>` | `ElementCount` key/value assignments |

### Convention-Based String Mapping

`string` properties — and `string` elements of collections and dictionaries — are matched by the property name's **suffix**, case-insensitive, singular or plural (`ContactEmail`, `Emails`):

| Suffix | Emitted |
| :--- | :--- |
| `Email` | `f.Internet.Email()` |
| `PhoneNumber`, `Phone`, `Mobile` | `f.Phone.PhoneNumber()` |
| `FirstName`, `GivenName` | `f.Name.FirstName()` |
| `LastName`, `Surname`, `FamilyName` | `f.Name.LastName()` |
| `UserName`, `Username` | `f.Internet.UserName()` |
| `Url`, `Website` | `f.Internet.Url()` |
| `PostalCode`, `ZipCode`, `Zip` | `f.Address.ZipCode()` |
| `City` | `f.Address.City()` |
| `Country` | `f.Address.Country()` |
| `CompanyName`, `Company` | `f.Company.CompanyName()` |

Mapping is performed by the built-in `string` provider, so a custom `string` provider registered ahead of it replaces conventions entirely.

### Collection & Dictionary Support

`T[]`, `List<T>`, `HashSet<T>` and `Dictionary<TKey, TValue>` are generated element-by-element using the same provider resolution as scalar properties, including nested collections of supported types.

### Namespace-Aware Output

A `using` directive is emitted only when the generated code names a type from that namespace. Collection element types, enums and custom provider namespaces are included; the source type's own namespace and duplicates are skipped.

### Syntax Validation

Every generated Faker is parsed before it is returned. A syntax error — for example from a custom provider expression — throws `ApplicationException` instead of producing a broken file.

### Graceful Fallback

A property that no provider can resolve is emitted as a comment, so nothing is guessed and nothing is dropped silently:

```csharp
// TODO: Address (Address)
```

---

## Extensible Architecture

Register:

* Custom `IDataTypeProvider` implementations
* Custom `IOptions` implementations

### Custom Type Providers

#### Example: Custom Provider

Assuming `Contoso.Billing.Money` exposes `Money(decimal amount, string currency)`:

```csharp
using System.Collections.Generic;
using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;
using Microlens.Synthesizer.Core.Providers;

public sealed class MoneyProvider : IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.Name == "Money" && metadata.Type.ContainingNamespace.ToDisplayString() == "Contoso.Billing";
    }

    public string Generate(PropertyMetadata metadata) {
        return "new Money(f.Finance.Amount(), f.Finance.Currency().Code)";
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        return metadata.Type.GetReferencedNamespaces();
    }
}
```

`Generate` returns the expression placed after `f =>`. Implement `INamespaceProvider` whenever that expression names a type.

#### Register: Custom Provider

```csharp
services.AddSingleton<IDataTypeProvider, MoneyProvider>();
```

Resolution rules:

* Providers are evaluated in registration order — the first `CanHandle` match wins.
* A provider registered **before** `AddCoreServices()` overrides built-in handling, including `string` conventions.
* A provider registered **after** `AddCoreServices()` only receives types no built-in provider handles.
* Register providers as singletons — the property generator is a singleton.
* Custom types inside supported collections and dictionaries (`List<Money>`) resolve through the same providers.

#### Helpers

| Extension | Purpose |
| :--- | :--- |
| `ITypeSymbol.ToDisplayName()` | Minimally qualified type name, as used in generated code. |
| `ITypeSymbol.GetReferencedNamespaces()` | Namespaces required to name the type, including type arguments and array elements. |

---

## Comparison

| Capability | `Microlens.Synthesizer.Core` | Manual `Faker<T>` Authoring |
| :--- | :--- | :--- |
| Property discovery (including inherited) | Automatic, via `SemanticModel` | Manual |
| Convention-based mapping (`Email`, `PhoneNumber`, etc.) | Yes | Manual |
| Collection / dictionary element rules | Generated automatically | Manual |
| Generic source types | Type parameters and constraints carried over | Manual |
| Namespace resolution for generated `using` directives | Automatic | Manual |
| Stays current after a class changes | Regenerate on demand | Manual re-sync |
| Unsupported property handling | Marked `// TODO:` | N/A |
| Custom type support | Extensible via `IDataTypeProvider` | Hand-written |

**`Microlens.Synthesizer.Core`** focuses on **generating** ordinary `Faker<T>` source — the output is a normal class you own, not a runtime-only construct.

---

## Limitations

* **One type per file**: Only the first non-static, top-level class or record class is processed; nested types cannot be targeted.
* **Same assembly**: Generated Fakers are assumed to compile into the source type's assembly (`internal` members are included).
* **Unsupported types** (emitted as `// TODO:`):
  * `sbyte`, `ushort`, `uint`, `ulong`
  * `TimeSpan`, `DateOnly`, `TimeOnly`, `Uri`, `object`
  * Type-parameter-typed properties (`T`)
  * Complex types (no child Faker composition)
  * `IEnumerable<T>`, `IList<T>`, `ICollection<T>`, `IReadOnly*`, `IDictionary<TKey, TValue>`
  * Collections of nullable elements (`List<int?>`)
* **Runtime instantiation**: `Faker<T>` uses a parameterless constructor by default. Abstract classes, positional records and types without one compile, but require `CustomInstantiator` before `Generate()`.
* **Unsupported declarations**: `struct` and `record struct` are not supported.
* **Collisions**: `HashSet<T>` and `Dictionary<TKey, TValue>` can hold fewer than `ElementCount` entries when generated values collide.
* **Conventions are suffix-based**: e.g. `Microphone` maps to a phone number.

---

## When Not To Use `Microlens.Synthesizer.Core`

`Microlens.Synthesizer.Core` is not intended for:

* Runtime/reflection-based fake generation with no persisted source file
* Serializing or deserializing real instances of your classes
* Types the built-in providers can't resolve, without registering a custom `IDataTypeProvider`
* Types with no instance, non-indexer properties that have an accessible getter and a setter

If you need fake data generated at runtime without a compiled `Faker<T>` class, a reflection-based library is a better fit.

---

## License

Licensed under the **Apache License 2.0**.

---
