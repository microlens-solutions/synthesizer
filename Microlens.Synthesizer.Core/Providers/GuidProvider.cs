using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class GuidProvider : ProviderBase, IDataTypeProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata.Type.IsSystemType("Guid");
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate("Random.Guid");
        }
    }
}
