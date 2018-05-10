namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Net.Model;

    public interface IModelFactory
    {
        NetAssembly CreateAssemblyModel(AssemblyKey key);

        NetType CreateTypeModel(TypeKey key);

        NetMethod CreateMethodModel(MethodKey key);

        NetType CreateGenericTypeArg(TypeKey key, string typeArgName);

        IEnumerable<NetAssembly> GetAssemblyModels();
        
        IEnumerable<NetType> GetTypeModels();
        
        IEnumerable<NetMethod> GetMethodModels();
    }
}