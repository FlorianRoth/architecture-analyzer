
namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    public interface IGenericContext
    {
        NetType DeclaringType { get; }

        IReadOnlyList<NetType> GenericParameters { get; }
    }
}
