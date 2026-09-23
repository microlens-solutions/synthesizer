using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class EnumProvider : ProviderBase, IDataTypeProvider, INamespaceProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata.Type.TypeKind == TypeKind.Enum;
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate($"PickRandom<{metadata.Type.ToDisplayName()}>");
        }

        public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
            return metadata.Type.GetReferencedNamespaces();
        }
    }
}
