using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class FloatProvider : ProviderBase, IDataTypeProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata.Type.SpecialType == SpecialType.System_Single;
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate("Random.Float");
        }
    }
}
