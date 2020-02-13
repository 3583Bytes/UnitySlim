using System;
using System.Collections.Generic;
using System.Text;

namespace Slim.Model
{
    public class LogItem : IEquatable<LogItem>, IComparer<LogItem>
    {
        public string ItemPath { get; set; }
        public float ItemSize { get; set; }
        public float ItemPercent { get; set; }

        public LogItem()
        {

        }
        public LogItem (string itemPath, float itemSize, float itemPercent)
        {
            ItemPath = itemPath;
            ItemSize = itemSize;
            ItemPercent = itemPercent;
        }


        public bool Equals(LogItem other)
        {
            if (other == null) return false;
            return (this.ItemPath.Equals(other.ItemPath));
        }

        public int Compare(LogItem x, LogItem y)
        {
            return y.ItemSize.CompareTo(x.ItemSize);
        }
    }
}
