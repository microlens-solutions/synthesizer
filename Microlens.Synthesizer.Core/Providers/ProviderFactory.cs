using Microlens.Synthesizer.Core.Metadata;
using Microlens.Synthesizer.Core.Options;
using System;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public static class ProviderFactory {
    public static IReadOnlyList<IPropertyRuleProvider> Provide(BogusOptions options, Func<PropertyMetadata, string?> generateExpression, Func<PropertyMetadata, IEnumerable<string>> getRequiredNamespaces) {
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
            new GuidRuleProvider(),
            new EnumRuleProvider(),
            new CollectionRuleProvider(options, generateExpression, getRequiredNamespaces)
        ];
    }
}
