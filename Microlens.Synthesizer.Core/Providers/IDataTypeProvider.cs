using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public interface IDataTypeProvider {
    bool CanHandle(PropertyMetadata metadata);

    string Generate(PropertyMetadata metadata);
}
