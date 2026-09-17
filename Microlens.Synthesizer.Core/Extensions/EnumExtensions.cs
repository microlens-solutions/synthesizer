using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Microlens.Synthesizer.Core.Extensions {
    public static class EnumExtensions {
        private static readonly ConcurrentDictionary<Enum, string> _descriptions = new ConcurrentDictionary<Enum, string>();

        public static string GetDescription<TEnum>(this TEnum key) where TEnum : struct, Enum {
            return _descriptions.GetOrAdd(key, value => {
                FieldInfo field = value.GetType().GetField(value.ToString());

                if (field == null) {
                    return value.ToString();
                }

                DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
                return attribute != null ? attribute.Description : value.ToString();
            });
        }
    }
}
