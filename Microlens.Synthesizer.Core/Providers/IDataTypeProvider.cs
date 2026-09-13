using Microlens.Synthesizer.Core.Domain;

namespace Microlens.Synthesizer.Core.Providers;

public interface IDataTypeProvider {
    bool CanHandle(PropertyMetadata metadata);

    string Generate(PropertyMetadata metadata);
}
