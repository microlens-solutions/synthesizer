namespace Microlens.Synthesizer.Core.Configurations {
    public interface IOptions {
        bool OverwriteExisting { get; }

        int ElementCount { get; }

        string FakerSuffix { get; }
    }
}
