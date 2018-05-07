namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    using System.Collections.Generic;

    public interface IModelFactory
    {
        NetAssembly CreateAssemblyModel(string id);

        NetType CreateTypeModel(string id);

        NetMethod CreateMethodModel(string id);

        IEnumerable<NetAssembly> GetAssemblyModels();
        
        IEnumerable<NetType> GetTypeModels();
        
        IEnumerable<NetMethod> GetMethodModels();
    }
}