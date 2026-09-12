using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Providers;
using Microlens.Synthesizer.Core.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Generators;

public sealed class PropertyGenerator(IEnumerable<IDataTypeProvider> providers) : IPropertyGenerator {
    public string Generate(PropertyMetadata metadata) {
        var expression = GenerateExpression(metadata);

        return expression is null
            ? $"// TODO: {metadata.Name} ({metadata.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)})"
            : $"RuleFor(x => x.{metadata.Name}, f => {expression});";
    }

    public string? GenerateExpression(PropertyMetadata metadata) {
        foreach (var provider in providers) {
            if (provider.CanHandle(metadata)) {
                return metadata.Type.SpecialType == SpecialType.System_String && TryGetFactory(metadata.Name, out var factory)
                    ? $"f.{factory}()"
                    : provider.Generate(metadata);
            }
        }

        return null;
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        foreach (var provider in providers) {
            if (!provider.CanHandle(metadata)) {
                continue;
            }

            if (provider is INamespaceProvider namespaces) {
                foreach (var ns in namespaces.GetRequiredNamespaces(metadata)) {
                    yield return ns;
                }
            }

            yield break;
        }
    }

    private static bool TryGetFactory(string propertyName, out string? factory) {
        foreach (var (suffixes, mappedFactory) in Registry.Mappings) {
            foreach (var suffix in suffixes) {
                if (propertyName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) || propertyName.EndsWith(suffix + "s", StringComparison.OrdinalIgnoreCase)) {
                    factory = mappedFactory;
                    return true;
                }
            }
        }

        factory = null;
        return false;
    }
}
