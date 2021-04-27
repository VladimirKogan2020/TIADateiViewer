using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbeaufgabeKogan
{
    public class GroupElement
    {
        public string Description { get; set; } = string.Empty;
        public int PropertyCount { get; set; } = 0;

        public GroupElement(string description,int propertyCount)
        {
            PropertyCount = propertyCount;
            Description = description;
        }
        public override string ToString()
        {
            return $"{Description} "+$"{PropertyCount}";
        }
    }
}
