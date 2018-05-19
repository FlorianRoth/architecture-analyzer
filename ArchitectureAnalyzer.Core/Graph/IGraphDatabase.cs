namespace ArchitectureAnalyzer.Core.Graph
{
    public interface IGraphDatabase
    {
        void Connect();

        void Disconnect();

        IGraphDatabaseTransaction BeginTransaction();
    }
}
