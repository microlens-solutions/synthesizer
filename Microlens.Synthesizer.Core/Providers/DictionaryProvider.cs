using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Resolvers;
using Microlens.Synthesizer.Core.Shared;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class DictionaryProvider(IExpressionResolver resolver) : IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return TryGetKeyValueTypes(metadata.Type, out var key, out var value) && resolver.GenerateExpression(new PropertyMetadata(metadata.Name, key)) is not null && resolver.GenerateExpression(new PropertyMetadata(metadata.Name, value)) is not null;
    }

    public string Generate(PropertyMetadata metadata) {
        _ = TryGetKeyValueTypes(metadata.Type, out var key, out var value);
        var (keyDisplay, keyExpression) = GetAttributes(metadata.Name, key);
        var (valueDisplay, valueExpression) = GetAttributes(metadata.Name, value);

        return $"{{ var map = new Dictionary<{keyDisplay}, {valueDisplay}>(); for (int i = 0; i < {Registry.ElementCount}; i++) {{ map[{keyExpression}] = {valueExpression}; }} return map; }}";
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        if (!TryGetKeyValueTypes(metadata.Type, out var key, out var value)) {
            yield break;
        }

        yield return "System.Collections.Generic";

        foreach (var ns in resolver.GetRequiredNamespaces(new PropertyMetadata(metadata.Name, key))) {
            yield return ns;
        }

        foreach (var ns in resolver.GetRequiredNamespaces(new PropertyMetadata(metadata.Name, value))) {
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

    private (string? Display, string? Expression) GetAttributes(string name, ITypeSymbol type) {
        var display = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var expression = resolver.GenerateExpression(new PropertyMetadata(name, type));

        return (display, expression);
    }
}
