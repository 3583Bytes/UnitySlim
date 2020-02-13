using System;


namespace Slim.Model
{
    public class SummaryItem : IEquatable<SummaryItem>
    {
        public string ItemType { get; set; }
        public float ItemSize { get; set; }

        public bool Equals(SummaryItem other)
        {
            if (other == null) return false;
            return (this.ItemType.Equals(other.ItemType));
        }

    }
}
