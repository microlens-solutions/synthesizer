using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;
using Microlens.Synthesizer.Core.Providers;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Generators {
    public sealed class PropertyGenerator : IPropertyGenerator {
        private readonly IEnumerable<IDataTypeProvider> _providers;

        public PropertyGenerator(IEnumerable<IDataTypeProvider> providers) {
            _providers = providers;
        }

        public string Generate(PropertyMetadata metadata) {
            var expression = GenerateExpression(metadata);

            return expression is null
                ? $"// TODO: {metadata.Name} ({metadata.Type.ToDisplayName()})"
                : $"RuleFor(x => x.{metadata.Name.ToIdentifier()}, f => {expression});";
        }

        public string GenerateExpression(PropertyMetadata metadata) {
            foreach (var provider in _providers) {
                if (provider.CanHandle(metadata)) {
                    return provider.Generate(metadata);
                }
            }

            return null;
        }

        public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
            foreach (var provider in _providers) {
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
    }
}
