namespace Microlens.Synthesizer.Core.Providers {
    public abstract class ProviderBase {
        protected string Generate(string valueFactory) {
            return $"f.{valueFactory}()";
        }
    }
}
