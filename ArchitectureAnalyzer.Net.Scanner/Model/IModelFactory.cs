namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Net.Model;

    public interface IModelFactory
    {
        NetAssembly CreateAssemblyModel(AssemblyKey key);

        NetType CreateTypeModel(TypeKey key);

        NetMethod CreateMethodModel(MethodKey key);

        NetMethodParameter CreateMethodParameter(MethodParameterKey key);

        NetType CreateGenericTypeArg(TypeKey key, string typeArgName);

        NetType CreateGenericParameter(MethodKey key, string typeArgName);

        IEnumerable<NetAssembly> GetAssemblyModels();
        
        IEnumerable<NetType> GetTypeModels();
        
        IEnumerable<NetMethod> GetMethodModels();

        IEnumerable<NetMethodParameter> GetMethodParameterModels();
    }
}