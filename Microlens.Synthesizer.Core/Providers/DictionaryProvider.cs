using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;
using Microlens.Synthesizer.Core.Options;
using Microlens.Synthesizer.Core.Resolvers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class DictionaryProvider : IDataTypeProvider, INamespaceProvider {
        private readonly IOptions _options;

        private readonly IExpressionResolver _resolver;

        public DictionaryProvider(IOptions options, IExpressionResolver resolver) {
            _options = options;
            _resolver = resolver;
        }

        public bool CanHandle(PropertyMetadata metadata) {
            return TryGetKeyValueTypes(metadata.Type, out var key, out var value) && _resolver.GenerateExpression(new PropertyMetadata(metadata.Name, key)) != null && _resolver.GenerateExpression(new PropertyMetadata(metadata.Name, value)) != null;
        }

        public string Generate(PropertyMetadata metadata) {
            _ = TryGetKeyValueTypes(metadata.Type, out var key, out var value);
            var (keyDisplay, keyExpression) = GetAttributes(metadata.Name, key);
            var (valueDisplay, valueExpression) = GetAttributes(metadata.Name, value);

            return $"{{ var map = new Dictionary<{keyDisplay}, {valueDisplay}>(); for (int i = 0; i < {_options.ElementCount}; i++) {{ map[{keyExpression}] = {valueExpression}; }} return map; }}";
        }

        public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
            if (!TryGetKeyValueTypes(metadata.Type, out var key, out var value)) {
                yield break;
            }

            yield return "System.Collections.Generic";

            foreach (var ns in key.GetReferencedNamespaces()) {
                yield return ns;
            }

            foreach (var ns in value.GetReferencedNamespaces()) {
                yield return ns;
            }

            foreach (var ns in _resolver.GetRequiredNamespaces(new PropertyMetadata(metadata.Name, key))) {
                yield return ns;
            }

            foreach (var ns in _resolver.GetRequiredNamespaces(new PropertyMetadata(metadata.Name, value))) {
                yield return ns;
            }
        }

        private static bool TryGetKeyValueTypes(ITypeSymbol type, out ITypeSymbol key, out ITypeSymbol value) {
            if (type is INamedTypeSymbol named && named.IsGenericType && named.TypeArguments.Length == 2) {
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

        private (string Display, string Expression) GetAttributes(string name, ITypeSymbol type) {
            var display = type.ToDisplayName();
            var expression = _resolver.GenerateExpression(new PropertyMetadata(name, type));

            return (display, expression);
        }
    }
}
