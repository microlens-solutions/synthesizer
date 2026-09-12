using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Generators;

public interface IBogusGenerator {
    string Generate(ClassMetadata metadata);
}
