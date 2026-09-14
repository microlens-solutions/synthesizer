using Microlens.Synthesizer.Core.Domain;
using Microsoft.CodeAnalysis;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class DecimalProvider : ProviderBase, IDataTypeProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata.Type.SpecialType == SpecialType.System_Decimal;
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate("Finance.Amount");
        }
    }
}
