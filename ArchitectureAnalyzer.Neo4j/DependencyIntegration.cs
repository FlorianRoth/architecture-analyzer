
namespace ArchitectureAnalyzer.Neo4j
{
    using System;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Neo4j.Graph;

    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyIntegration
    {
        public static void AddNeo4J(this IServiceCollection services, Action<Neo4JConfiguration> configure)
        {
            var config = new Neo4JConfiguration();
            configure(config);

            services.AddSingleton(config);
            services.AddSingleton<IGraphDatabase, Neo4jDatabase>();
        }
    }
}
