namespace Microlens.Synthesizer.Core.Options;

public sealed class BogusOptions {
    public string FakerSuffix { get; set; } = "Faker";

    public bool OverwriteExisting { get; set; } = true;

    public string OutputExtension { get; set; } = ".cs";
}
