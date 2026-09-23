using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Extensions;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class DateTimeOffsetProvider : ProviderBase, IDataTypeProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata.Type.IsSystemType("DateTimeOffset");
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate("Date.PastOffset");
        }
    }
}
