using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public static class ProviderFactory {
    public static IReadOnlyList<IPropertyRuleProvider> Provide() {
        return [
            new StringRuleProvider(),
            new IntegerRuleProvider(),
            new GuidRuleProvider()
        ];
    }
}
