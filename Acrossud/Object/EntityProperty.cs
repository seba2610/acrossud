using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Object
{
    public class EntityProperty
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int PropertyId { get; set; }
        public object Value { get; set; }
    }
}
