using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Options;
using Microlens.Synthesizer.Core.Resolvers;
using Microlens.Synthesizer.Core.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class CollectionProvider : IDataTypeProvider, INamespaceProvider {
        private readonly IOptions _options;

        private readonly IExpressionResolver _resolver;

        public CollectionProvider(IOptions options, IExpressionResolver resolver) {
            _options = options;
            _resolver = resolver;
        }

        public bool CanHandle(PropertyMetadata metadata) {
            return TryGetElementType(metadata.Type, out var type, out _) && _resolver.GenerateExpression(new PropertyMetadata(metadata.Name, type)) != null;
        }

        public string Generate(PropertyMetadata metadata) {
            _ = TryGetElementType(metadata.Type, out var type, out var kind);

            var expression = _resolver.GenerateExpression(new PropertyMetadata(metadata.Name, type));
            var display = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            var make = $"f.Make({_options.ElementCount}, () => {expression})";

            switch (kind) {
                case Registry.CollectionKind.Array:
                    return $"new List<{display}>({make}).ToArray()";

                case Registry.CollectionKind.HashSet:
                    return $"new HashSet<{display}>({make})";

                case Registry.CollectionKind.List:
                    return $"new List<{display}>({make})";

                default:
                    throw new ApplicationException($"Unsupported collection kind for '{metadata.Name}'.");
            }
        }

        public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
            if (!TryGetElementType(metadata.Type, out var type, out _)) {
                yield break;
            }

            yield return "System.Collections.Generic";

            foreach (var ns in _resolver.GetRequiredNamespaces(new PropertyMetadata(metadata.Name, type))) {
                yield return ns;
            }
        }

        private static bool TryGetElementType(ITypeSymbol input, out ITypeSymbol type, out Registry.CollectionKind kind) {
            if (input is IArrayTypeSymbol arrayType) {
                type = arrayType.ElementType;
                kind = Registry.CollectionKind.Array;

                return true;
            }

            if (input is INamedTypeSymbol named && named.IsGenericType && named.TypeArguments.Length == 1) {
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
}
