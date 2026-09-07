using Microlens.Synthesizer.Core.Metadata;

namespace Microlens.Synthesizer.Core.Providers;

public interface INamespaceAwareRuleProvider {
    string? GetRequiredNamespace(PropertyMetadata metadata);
}
