namespace ArchitectureAnalyzer.Neo4j.Graph
{
    using ArchitectureAnalyzer.Core.Graph;

    using Microsoft.Extensions.Logging;

    using Neo4jClient;

    internal class Neo4jDatabase : IGraphDatabase
    {
        private readonly ILogger<Neo4jDatabase> _logger;

        private readonly IGraphClient _client;

        public Neo4jDatabase(
            Neo4JConfiguration config,
            ILogger<Neo4jDatabase> logger)
        {
            _logger = logger;

            _client = new GraphClient(
                config.Host,
                config.User,
                config.Password);
        }

        public void Connect()
        {
            _logger.LogInformation("Connecting to graph database");

            _client.Connect();
        }

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting from graph database");

            _client.Dispose();
        }

        public void Clear()
        {
            _logger.LogInformation("Clearing database");
            
            _client.Cypher
                .Match("(n)")
                .OptionalMatch("(n)-[r]-()")
                .Delete("n, r")
                .ExecuteWithoutResults();
        }

        public void CreateNode<T>(T model)
            where T : Node
        {
            const string PARAM = "model";
            var label = typeof(T).Name;

            _client.Cypher
                .Create($"(:{label} {{{PARAM}}})")
                .WithParam(PARAM, model)
                .ExecuteWithoutResults();
        }

        public void CreateRelationship<TFrom, TTo>(TFrom fromNode, TTo toNode, string relationType)
            where TFrom : Node where TTo : Node
        {
            var fromLabel = typeof(TFrom).Name;
            var toLabel = typeof(TTo).Name;

            var fromId = fromNode.Id;
            var toId = toNode.Id;

           _client.Cypher
                .Match($"(from:{fromLabel})", $"(to:{toLabel})")
                .Where((TFrom from) => from.Id == fromId)
                .AndWhere((TTo to) => to.Id == toId)
                .Create($"(from)-[:{relationType}]->(to)")
                .ExecuteWithoutResults();
        }
    }
}
