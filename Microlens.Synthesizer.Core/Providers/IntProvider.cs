using Microlens.Synthesizer.Core.Metadata;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class IntProvider : ProviderBase, IDataTypeProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.SpecialType == SpecialType.System_Int32;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Random.Int");
    }
}
