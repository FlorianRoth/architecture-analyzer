namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    using System.Collections.Generic;

    public interface IModelFactory
    {
        NetAssembly CreateAssemblyModel(AssemblyKey key);

        NetType CreateTypeModel(TypeKey key);

        NetMethod CreateMethodModel(MethodKey key);

        IEnumerable<NetAssembly> GetAssemblyModels();
        
        IEnumerable<NetType> GetTypeModels();
        
        IEnumerable<NetMethod> GetMethodModels();
    }
}