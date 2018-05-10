
namespace ArchitectureAnalyzer.Net.Model
{
    public sealed class HasParameterRelationship
    {
        public int Order { get; }

        public HasParameterRelationship(int order)
        {
            Order = order;
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is HasParameterRelationship other)
            {
                return Order == other.Order;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Order;
        }
    }
}
