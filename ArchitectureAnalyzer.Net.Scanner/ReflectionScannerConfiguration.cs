
namespace ArchitectureAnalyzer.Net.Scanner
{
    using System.Collections.Generic;

    public class ReflectionScannerConfiguration
    {
        public IList<string> Assemblies { get; set; }

        public ReflectionScannerConfiguration()
        {
            Assemblies = new List<string>();
        }
    }
}
