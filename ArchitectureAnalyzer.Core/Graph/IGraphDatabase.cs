namespace ArchitectureAnalyzer.Core.Graph
{
    public interface IGraphDatabase
    {
        void Connect();

        void Disconnect();

        void Clear();

        void CreateNode<T>(T model)
            where T : Node;

        void CreateRelationship<TFrom, TTo>(TFrom fromNode, TTo toNode, string relationType)
            where TFrom : Node
            where TTo : Node;

        void CreateRelationship<TFrom, TTo, TRel>(TFrom fromNode, TTo toNode, string relationType, TRel relationshipProperties)
            where TFrom : Node
            where TTo : Node;
    }
}
