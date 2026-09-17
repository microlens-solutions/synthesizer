# Microlens.Synthesizer.Core

The Roslyn-based generation engine behind **Generate Bogus Fakers** — turns a C# class or record into the full source of a [Bogus](https://github.com/bchavez/Bogus) `Faker<T>`, from a semantic model, with no reflection or runtime type inspection.

## Prefer the in-IDE experience?
Most people don't need this package directly — install the Visual Studio extension and generate Fakers by right-clicking a file, folder, or project:

- **Generate Bogus Fakers** (Visual Studio 2022) — _(add Marketplace link)_
- **Generate Bogus Fakers** (Visual Studio 2019) — _(add Marketplace link)_

Use `Microlens.Synthesizer.Core` directly only if you need the same generation logic outside Visual Studio — a CI code-gen step, an MSBuild task, a source generator, or your own tooling.

## What it does
Given a Roslyn `SemanticModel` positioned on a class or record declaration, it extracts the settable properties and emits a complete, compilable `Faker<T>` class as a string.

## Supported types

| Category | Types |
|---|---|
| Scalars | `bool`, `char`, `byte`, `short`, `int`, `long`, `float`, `double`, `decimal` |
| Text | `string` |
| Date/time | `DateTime`, `DateTimeOffset` |
| Identifiers | `Guid` |
| Enums | `enum` |
| Collections | `T[]`, `List<T>`, `HashSet<T>` |
| Maps | `Dictionary<TKey, TValue>` |

`Nullable` value types are unwrapped automatically. Only settable, non-static, non-indexer properties are included.

## Convention-based property mapping
Property names are matched against common naming conventions before falling back to the type-based provider:

- `Email` → email address
- `FirstName` / `GivenName`, `LastName` / `Surname` / `FamilyName` → real names
- `PhoneNumber` / `Phone` / `Mobile` → phone numbers
- `UserName` / `Username`, `Url` / `Website` → internet data
- `City`, `Country`, `PostalCode` / `ZipCode` / `Zip` → addresses
- `CompanyName` / `Company` → company names

Anything that can't be confidently mapped is emitted as a `// TODO:` comment instead of a silent guess.

## Install

```csharp
dotnet add package Microlens.Synthesizer.Core
```

## Usage

```csharp
using Microlens.Synthesizer.Core.Analyzers;
using Microlens.Synthesizer.Core.Extensions;
using Microlens.Synthesizer.Core.Generators;
using Microlens.Synthesizer.Core.Options;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

// 1. Register services (bring your own IOptions implementation)
var services = new ServiceCollection();
services.AddCoreServices();
services.AddSingleton<IOptions>(new MyGeneratorOptions());
var provider = services.BuildServiceProvider();

// 2. Get a Roslyn SemanticModel for the class/record you want a Faker for
var tree = CSharpSyntaxTree.ParseText(sourceText);
var compilation = CSharpCompilation.Create("Analysis", new[] { tree }, references); // your own MetadataReferences
var model = compilation.GetSemanticModel(tree);

// 3. Analyze the type
var analyzer = provider.GetRequiredService<IClassAnalyzer>();
var metadata = analyzer.Analyze(model);

// 4. Generate the Faker<T> source
var generator = provider.GetRequiredService<IBogusGenerator>();
string fakerSource = generator.Generate(metadata);
```

## Configuration — `IOptions`
The package ships no default implementation — implement and register your own:

```csharp
public interface IOptions {
    bool OverwriteExisting { get; }
    int ElementCount { get; }
    string FakerSuffix { get; }
    string LogFilePath { get; }
    Registry.LogWritingMode LogWritingMode { get; }
}
```

| Property | Purpose |
|---|---|
| `OverwriteExisting` | Overwrite an existing generated file, or skip if one is present |
| `ElementCount` | Number of elements generated for arrays, lists, sets, and dictionaries |
| `FakerSuffix` | Suffix appended to the source class name for the generated class and file name |
| `LogFilePath` / `LogWritingMode` | Only relevant if you also use `IFileWriter`'s built-in logging conventions |

## Writing output to disk
Optional — `IFileWriter` handles the overwrite policy for you:

```csharp
var writer = provider.GetRequiredService<IFileWriter>();

if (writer.TryWrite(sourceFileName, outputDirectory, fakerSource, out var outputPath)) {
    // outputPath now points to the generated {ClassName}{FakerSuffix}.cs file
}
```

## Requirements
- .NET Standard 2.0 — compatible with .NET Framework 4.6.1+, .NET Core 2.0+, and .NET 5+
- Depends on `Microsoft.CodeAnalysis.CSharp` and `Microsoft.Extensions.DependencyInjection`

## License
See the license included with this package.
