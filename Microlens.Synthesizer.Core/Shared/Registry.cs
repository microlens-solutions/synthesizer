using System;

namespace Microlens.Synthesizer.Core.Shared;

public static class Registry {
    public static readonly string ApplicationName = "Microlens Synthesizer";

    public static readonly int CommandId = 0x0100;

    public static readonly Guid CommandSet = new("8A6F8B16-7E2A-4C1B-9E7A-2E8A9B2C5F01");

    public enum CollectionKind {
        Array,

        List,

        HashSet
    }
}
