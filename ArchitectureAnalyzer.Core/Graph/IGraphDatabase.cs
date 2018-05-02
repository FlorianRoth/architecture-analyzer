namespace ArchitectureAnalyzer.Core.Graph
{
    public interface IGraphDatabase
    {
        void Connect();

        void Disconnect();

        void Clear();

        void CreateNode<T>(T model)
            where T : Node;

        void CreateRelationship<TFrom, TTo>(TFrom from, TTo to, string relationType)
            where TFrom : Node
            where TTo : Node;
    }
}
