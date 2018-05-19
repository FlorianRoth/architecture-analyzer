namespace ArchitectureAnalyzer.Neo4j.Graph
{
    using ArchitectureAnalyzer.Core.Graph;

    using global::Neo4j.Driver.V1;

    using Microsoft.Extensions.Logging;
    
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Neo4JDatabase : IGraphDatabase
    {
        private readonly Neo4JConfiguration _config;

        private readonly ILogger<Neo4JDatabase> _logger;

        private IDriver _driver;

        public Neo4JDatabase(
            Neo4JConfiguration config,
            ILogger<Neo4JDatabase> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Connect()
        {
            _logger.LogInformation("Connecting to graph database");
            
            var authToken = AuthTokens.Basic(_config.User, _config.Password);
            _driver = GraphDatabase.Driver(_config.Host, authToken);
        }

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting from graph database");

            _driver?.Dispose();
        }

        public IGraphDatabaseTransaction BeginTransaction()
        {
            return new Neo4JTransaction(_driver);
        }
    }
}
