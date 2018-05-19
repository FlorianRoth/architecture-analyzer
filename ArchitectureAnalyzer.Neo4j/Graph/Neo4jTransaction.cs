
namespace ArchitectureAnalyzer.Neo4j.Graph
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;

    using global::Neo4j.Driver.V1;
    
    internal class Neo4JTransaction : IGraphDatabaseTransaction
    {
        private readonly ISession _session;

        private readonly ITransaction _tx;

        public Neo4JTransaction(IDriver driver)
        {
            _session = driver.Session();
            _tx = _session.BeginTransaction();
        }

        public void Dispose()
        {
            _tx.Dispose();
            _session.Dispose();
        }

        public void Commit()
        {
            _tx.Success();
        }

        public void Clear()
        {
            var statement = new Statement("MATCH (n) DETACH DELETE n");

            _tx.Run(statement);
        }

        public void CreateNode<T>(T model)
            where T : Node
        {
            var label = typeof(T).Name;

            var statement = $"CREATE (:{label} {{model}})";

            var modelData = ModelConverter.Convert(model);
            var parameters = new Dictionary<string, object> { { "model", modelData } };

            _tx.Run(statement, parameters);
        }

        public void CreateRelationship<TFrom, TTo>(TFrom fromNode, TTo toNode, string relationType)
            where TFrom : Node where TTo : Node
        {
            var fromLabel = typeof(TFrom).Name;
            var toLabel = typeof(TTo).Name;

            var fromId = fromNode.Id;
            var toId = toNode.Id;

            var statement = $"MATCH (from:{fromLabel} {{ Id: {{fromId}} }}), (to:{toLabel}  {{ Id: {{toId}} }})"
                            //+ $" WHERE from.Id = {{fromId}} AND to.Id = {{toId}}"
                            + $" CREATE (from)-[:{relationType}]->(to)";

            var parameters = new Dictionary<string, object> { { "fromId", fromId }, { "toId", toId } };

            _tx.Run(statement, parameters);
        }
    }
}
