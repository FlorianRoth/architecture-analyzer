namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    public interface IModelFactory
    {
        NetAssembly CreateAssemblyModel(string id);

        NetType CreateTypeModel(string id);

        NetMethod CreateMethodModel(string id);
    }
}