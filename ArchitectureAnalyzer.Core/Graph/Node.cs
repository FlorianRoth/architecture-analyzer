
namespace ArchitectureAnalyzer.Core.Graph
{
    public abstract class Node
    {
        public long Id { get; set; }
        

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Node)obj;

            return Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
