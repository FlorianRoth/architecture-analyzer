namespace ArchitectureAnalyzer.Net.Model
{
    using System;

    [Flags]
    public enum Visibility
    {
        Public,
        Internal,
        Protected,
        Private
    }
}