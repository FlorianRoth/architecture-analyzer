namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Net.Model;

    using Mono.Cecil;

    public interface IModelFactory
    {
        NetAssembly CreateAssemblyModel(AssemblyDefinition assemblyDefinition);

        NetAssembly CreateAssemblyModel(AssemblyNameReference assemblyReference);
        
        NetType CreateTypeModel(TypeReference typeReference);

        NetMethod CreateMethodModel(MethodDefinition methodDefinition);

        NetMethodParameter CreateMethodParameter(MethodDefinition method, ParameterDefinition param);

        NetType CreateGenericTypeArg(GenericParameter parameter);
        
        NetProperty CreatePropertyModel(PropertyDefinition propertyDefinition);


        IEnumerable<NetAssembly> GetAssemblyModels();
        
        IEnumerable<NetType> GetTypeModels();
        
        IEnumerable<NetMethod> GetMethodModels();

        IEnumerable<NetMethodParameter> GetMethodParameterModels();

        IEnumerable<NetProperty> GetPropertyModels();
    }
}