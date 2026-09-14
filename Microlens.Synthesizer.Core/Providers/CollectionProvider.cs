using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Resolvers;
using Microlens.Synthesizer.Core.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class CollectionProvider(IExpressionResolver resolver) : IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return TryGetElementType(metadata.Type, out var type, out _) && resolver.GenerateExpression(new PropertyMetadata(metadata.Name, type)) is not null;
    }

    public string Generate(PropertyMetadata metadata) {
        _ = TryGetElementType(metadata.Type, out var type, out var kind);

        var expression = resolver.GenerateExpression(new PropertyMetadata(metadata.Name, type));
        var made = $"f.Make({Registry.ElementCount}, () => {expression})";

        return kind switch {
            Registry.CollectionKind.Array => $"{made}.ToArray()",
            Registry.CollectionKind.HashSet => $"new HashSet<{type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>({made})",
            Registry.CollectionKind.List => made,
            _ => throw new ApplicationException($"Unsupported collection kind for '{metadata.Name}'.")
        };
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        if (!TryGetElementType(metadata.Type, out var type, out var kind)) {
            yield break;
        }

        if (kind == Registry.CollectionKind.HashSet) {
            yield return "System.Collections.Generic";
        }

        foreach (var ns in resolver.GetRequiredNamespaces(new PropertyMetadata(metadata.Name, type))) {
            yield return ns;
        }
    }

    private static bool TryGetElementType(ITypeSymbol input, out ITypeSymbol type, out Registry.CollectionKind kind) {
        if (input is IArrayTypeSymbol arrayType) {
            type = arrayType.ElementType;
            kind = Registry.CollectionKind.Array;

            return true;
        }

        if (input is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } named) {
            var definition = named.OriginalDefinition;
            var definitionNamespace = definition.ContainingNamespace.ToDisplayString();

            if (definition.Name == "List" && definitionNamespace == "System.Collections.Generic") {
                type = named.TypeArguments[0];
                kind = Registry.CollectionKind.List;
                return true;
            }

            if (definition.Name == "HashSet" && definitionNamespace == "System.Collections.Generic") {
                type = named.TypeArguments[0];
                kind = Registry.CollectionKind.HashSet;
                return true;
            }
        }

        type = input;
        kind = default;
        return false;
    }
}
