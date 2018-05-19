namespace ArchitectureAnalyzer.Core.Graph
{
    using System;

    public interface IGraphDatabaseTransaction : IDisposable
    {
        void Commit();

        void Clear();

        void CreateNode<T>(T model)
            where T : Node;

        void CreateRelationship<TFrom, TTo>(TFrom fromNode, TTo toNode, string relationType)
            where TFrom : Node
            where TTo : Node;
    }
}
