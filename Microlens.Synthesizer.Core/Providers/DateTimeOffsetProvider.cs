using Microlens.Synthesizer.Core.Domain;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class DateTimeOffsetProvider : ProviderBase, IDataTypeProvider, INamespaceProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata?.Type?.ContainingNamespace != null
                && string.Equals(metadata.Type.Name, "DateTimeOffset", StringComparison.OrdinalIgnoreCase)
                && string.Equals(metadata.Type.ContainingNamespace.Name, "System", StringComparison.OrdinalIgnoreCase);
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate("Date.PastOffset");
        }

        public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
            yield return "System";
        }
    }
}
