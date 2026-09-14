using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Microlens.Synthesizer.Core.Providers;

public sealed class DateTimeProvider : ProviderBase, IDataTypeProvider, INamespaceProvider {
    public bool CanHandle(PropertyMetadata metadata) {
        return metadata.Type.SpecialType == SpecialType.System_DateTime;
    }

    public string Generate(PropertyMetadata metadata) {
        return Generate("Date.Past");
    }

    public IEnumerable<string> GetRequiredNamespaces(PropertyMetadata metadata) {
        yield return "System";
    }
}
