using Microlens.Synthesizer.Core.Domain;
using Microlens.Synthesizer.Core.Shared;
using Microsoft.CodeAnalysis;
using System;

namespace Microlens.Synthesizer.Core.Providers {
    public sealed class StringProvider : ProviderBase, IDataTypeProvider {
        public bool CanHandle(PropertyMetadata metadata) {
            return metadata.Type.SpecialType == SpecialType.System_String;
        }

        public string Generate(PropertyMetadata metadata) {
            return Generate(TryGetFactory(metadata.Name, out var factory) ? factory : "Name.FullName");
        }

        private static bool TryGetFactory(string property, out string factory) {
            foreach (var (suffixes, mappedFactory) in Registry.Mappings) {
                foreach (var suffix in suffixes) {
                    if (property.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) || property.EndsWith($"{suffix}s", StringComparison.OrdinalIgnoreCase)) {
                        factory = mappedFactory;
                        return true;
                    }
                }
            }

            factory = null;
            return false;
        }
    }
}
