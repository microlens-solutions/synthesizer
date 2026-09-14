using Microlens.Synthesizer.Core.Analyzers;
using Microlens.Synthesizer.Core.Generators;
using Microlens.Synthesizer.Core.Persistence;
using Microlens.Synthesizer.Core.Providers;
using Microlens.Synthesizer.Core.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microlens.Synthesizer.Core.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddCoreServices(this IServiceCollection services) {
            _ = services.AddSingleton<IClassAnalyzer, ClassAnalyzer>();
            _ = services.AddSingleton<IFileWriter, FileWriter>();

            _ = services.AddSingleton<IExpressionResolver, ExpressionResolver>();
            _ = services.AddSingleton(sp => new Lazy<IPropertyGenerator>(() => sp.GetRequiredService<IPropertyGenerator>()));
            _ = services.AddSingleton<IPropertyGenerator, PropertyGenerator>();
            _ = services.AddSingleton<IBogusGenerator, BogusGenerator>();

            _ = services.AddSingleton<IDataTypeProvider, BoolProvider>();
            _ = services.AddSingleton<IDataTypeProvider, CharProvider>();
            _ = services.AddSingleton<IDataTypeProvider, StringProvider>();
            _ = services.AddSingleton<IDataTypeProvider, ByteProvider>();
            _ = services.AddSingleton<IDataTypeProvider, ShortProvider>();
            _ = services.AddSingleton<IDataTypeProvider, IntProvider>();
            _ = services.AddSingleton<IDataTypeProvider, LongProvider>();
            _ = services.AddSingleton<IDataTypeProvider, FloatProvider>();
            _ = services.AddSingleton<IDataTypeProvider, DoubleProvider>();
            _ = services.AddSingleton<IDataTypeProvider, DecimalProvider>();
            _ = services.AddSingleton<IDataTypeProvider, DateTimeProvider>();
            _ = services.AddSingleton<IDataTypeProvider, DateTimeOffsetProvider>();
            _ = services.AddSingleton<IDataTypeProvider, GuidProvider>();
            _ = services.AddSingleton<IDataTypeProvider, EnumProvider>();
            _ = services.AddSingleton<IDataTypeProvider, CollectionProvider>();
            _ = services.AddSingleton<IDataTypeProvider, DictionaryProvider>();

            return services;
        }
    }
}
