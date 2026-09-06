using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public static class ProviderFactory {
    public static IReadOnlyList<IPropertyRuleProvider> Provide() {
        return [
            new BoolRuleProvider(),
            new CharRuleProvider(),
            new StringRuleProvider(),
            new ByteRuleProvider(),
            new ShortRuleProvider(),
            new IntRuleProvider(),
            new LongRuleProvider(),
            new FloatRuleProvider(),
            new DoubleRuleProvider(),
            new DecimalRuleProvider(),
            new DateTimeRuleProvider(),
            new DateTimeOffsetRuleProvider(),
            new GuidRuleProvider()
        ];
    }
}
