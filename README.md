# `Microlens.Synthesizer.Core`

Generate compilable `Bogus` `Faker<T>` classes directly from `C#` classes and records, using the `Roslyn` semantic model — no reflection, no runtime type inspection, no hand-written boilerplate.

**`Microlens.Synthesizer.Core`** is a `Roslyn`-based `Faker<T>` generation engine for .NET that **analyzes**, **maps** and **emits** compilable `Bogus` fakers directly from a `SemanticModel`.

Unlike reflection-based fake-data generators that build values at runtime, **`Microlens.Synthesizer.Core`** emits an actual `Faker<T>` class as compilable `C#` source — reviewable, debuggable, and just as fast as a hand-written one, because it is one.

[Why `Microlens.Synthesizer.Core`?](#why-microlenssynthesizercore) | [Visual Studio Extension](#visual-studio-extension) | [Quick Start](#quick-start) | [Quick Example](#quick-example) | [Features](#features) | [Extensible Architecture](#extensible-architecture) | [Comparison](#comparison) | [When Not To Use `Microlens.Synthesizer.Core`](#when-not-to-use-microlenssynthesizercore) | [License](#license)

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

Most users won't need this package directly — the companion Visual Studio extension generates and saves `Faker<T>` files for you, right from Solution Explorer:

**Generate Bogus Fakers** extension is available for Visual Studio [2019](https://marketplace.visualstudio.com/items?itemName=microlens.legacysynthesizer), [2022](https://marketplace.visualstudio.com/items?itemName=microlens.modernsynthesizer) and [2026](https://marketplace.visualstudio.com/items?itemName=microlens.modernsynthesizer).

Use `Microlens.Synthesizer.Core` directly only when you need the same generation logic outside Visual Studio — a build step, a source generator, or your own tooling.

---

## Quick Start

### Installation

```bash
dotnet add package Microlens.Synthesizer.Core
```

### Register Services

```csharp
services.AddCoreServices();
services.AddSingleton<IOptions>(new MyGeneratorOptions());
```

`Microlens.Synthesizer.Core` ships no default `IOptions` implementation — `OverwriteExisting`, `ElementCount`, `FakerSuffix`, and the logging settings (`LogFilePath`, `LogWritingMode`) are entirely up to the host.

### Analyze And Generate

```csharp
var analyzer = provider.GetRequiredService<IClassAnalyzer>();
var metadata = analyzer.Analyze(semanticModel);

var generator = provider.GetRequiredService<IBogusGenerator>();
string fakerSource = generator.Generate(metadata);
```

That's it — `fakerSource` is a complete, compilable `Faker<T>` class as a string.

---

## Quick Example

Given a class:

```csharp
public class Customer {
    public string FirstName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public List<string> Tags { get; set; }
}
```

**`Microlens.Synthesizer.Core`** analyzes it through `Roslyn` and emits:

```csharp
using Bogus;

namespace YourNamespace {
    public class CustomerFaker : Faker<Customer> {
        public CustomerFaker() {
            RuleFor(x => x.FirstName, f => f.Name.FirstName());
            RuleFor(x => x.Email, f => f.Internet.Email());
            RuleFor(x => x.Age, f => f.Random.Int());
            RuleFor(x => x.Tags, f => new List<string>(f.Make(3, () => f.Name.FullName())));
        }
    }
}
```

* No manually written `RuleFor` calls.
* No missed properties.
* No stale Fakers after a property is renamed — just regenerate.

---

## Features

### Semantic-Model-Based Analysis

Property discovery runs against a `Roslyn` `SemanticModel`, not text or reflection — nested types, `records`, and `nullable value types` resolve correctly every time.

### Convention-Based Property Mapping

Property names are matched against common naming conventions before falling back to the type-based provider:

* `Email` → email address
* `FirstName` / `GivenName`, `LastName` / `Surname` / `FamilyName` → real names
* `PhoneNumber` / `Phone` / `Mobile` → phone numbers
* `UserName` / `Username`, `Url` / `Website` → internet data
* `City`, `Country`, `PostalCode` / `ZipCode` / `Zip` → addresses
* `CompanyName` / `Company` → company names

### Collection & Dictionary Support

`T[]`, `List<T>`, `HashSet<T>`, and `Dictionary<TKey, TValue>` are generated element-by-element using the same provider resolution as scalar properties, including nested collections of supported types.

### Namespace-Aware Output

Providers that need an extra `using` directive (`DateTime`, `Guid`, enums, collections, and so on) declare it themselves — the emitted file only imports what it actually needs.

### Graceful Fallback

A property, where the built-in providers could not be resolved, is emitted as a `// TODO:` comment instead of a silent guess, so nothing is generated incorrectly and nothing is generated invisibly.

---

## Extensible Architecture

Register:

* Custom `IDataTypeProvider` implementations
* Custom `IOptions` implementations

### Custom Type Providers

#### Example: Custom Provider

```csharp
public sealed class MoneyProvider : ProviderBase, IDataTypeProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.Name == "Money"; // domain-specific type
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Finance.Amount");
    }
}
```

#### Register: Custom Provider

```csharp
services.AddSingleton<IDataTypeProvider, MoneyProvider>();
```

Providers are resolved in registration order — the first `CanHandle` match wins. A provider that also implements `INamespaceProvider` contributes its own `using` directives to the generated file.

---

## Comparison

| Capability | `Microlens.Synthesizer.Core` | Manual `Faker<T>` Authoring |
|---|---|---|
| Property discovery | Automatic, via `SemanticModel` | Manual |
| Convention-based mapping (`Email`, `PhoneNumber`, etc.) | Yes | Manual |
| Collection / dictionary element rules | Generated automatically | Manual |
| Namespace resolution for generated `using` directives | Automatic | Manual |
| Stays current after a class changes | Regenerate on demand | Manual re-sync |
| Unsupported property handling | Marked `// TODO:` | N/A |
| Custom type support | Extensible via `IDataTypeProvider` | N/A |

**`Microlens.Synthesizer.Core`** focuses on **generating** ordinary `Faker<T>` source — the output is a normal class you own, not a runtime-only construct.

---

## When Not To Use `Microlens.Synthesizer.Core`

`Microlens.Synthesizer.Core` is not intended for:

* Runtime/reflection-based fake generation with no persisted source file
* Serializing or deserializing real instances of your classes
* Types the built-in providers can't resolve, without registering a custom `IDataTypeProvider`
* Types with no settable, non-static, non-indexer properties

If you need fake data generated at runtime without a compiled `Faker<T>` class, a reflection-based library is a better fit.

---

## License

Licensed under the **Apache License 2.0**.  
See the terms included with this package.

---
