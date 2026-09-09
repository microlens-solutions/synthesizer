using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Options;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class DictionaryRuleProvider(BogusOptions options, Func<PropertyMetadata, string?> generateExpression, Func<PropertyMetadata, IEnumerable<string>> getRequiredNamespaces) : IPropertyRuleProvider, INamespaceAwareRuleProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return TryGetKeyValueTypes(metadata.Type, out var key, out var value) && generateExpression(new PropertyMetadata(string.Empty, key)) is not null && generateExpression(new PropertyMetadata(string.Empty, value)) is not null;
    }

    public string Generate(PropertyMetadata metadata) {
        _ = TryGetKeyValueTypes(metadata.Type, out var key, out var value);
        var (keyName, keyExpression) = GetAttributes(key);
        var (valueName, valueExpression) = GetAttributes(value);

        return $"{{ var map = new Dictionary<{keyName}, {valueName}>(); for (int i = 0; i < {options.ElementCount}; i++) {{ map[{keyExpression}] = {valueExpression}; }} return map; }}";
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        if (!TryGetKeyValueTypes(metadata.Type, out var key, out var value)) {
            yield break;
        }

        yield return "System.Collections.Generic";

        foreach (var ns in getRequiredNamespaces(new PropertyMetadata(string.Empty, key))) {
            yield return ns;
        }

        foreach (var ns in getRequiredNamespaces(new PropertyMetadata(string.Empty, value))) {
            yield return ns;
        }
    }

    private static bool TryGetKeyValueTypes(ITypeSymbol type, out ITypeSymbol key, out ITypeSymbol value) {
        if (type is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 2 } named) {
            if (named.OriginalDefinition.Name == "Dictionary" && named.OriginalDefinition.ContainingNamespace.ToDisplayString() == "System.Collections.Generic") {
                key = named.TypeArguments[0];
                value = named.TypeArguments[1];
                return true;
            }
        }

        key = type;
        value = type;
        return false;
    }

    private (string? Name, string? Expression) GetAttributes(ITypeSymbol type) {
        var expression = generateExpression(new PropertyMetadata(string.Empty, type));
        var name = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        return (name, expression);
    }
}
