namespace Microlens.Synthesizer.Core.Providers;

public abstract class ProviderBase {
    protected string Generate(string propertyName, string valueFactory) {
        return $"RuleFor(x => x.{propertyName}, f => f.{valueFactory}());";
    }
}
