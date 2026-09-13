using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class BoolProvider : ProviderBase, IDataTypeProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.SpecialType == SpecialType.System_Boolean;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Random.Bool");
    }
}
