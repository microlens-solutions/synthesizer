using Microlens.Synthesizer.Core.Domain;

namespace Microlens.Synthesizer.Core.Generators {
    public interface IBogusGenerator {
        string Generate(ClassMetadata metadata);
    }
}
