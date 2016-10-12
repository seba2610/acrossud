using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrossud
{
    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public Entity(DataRow row)
        {
                Id = Int32.Parse(row["Id"].ToString());
                Name = row["Name"].ToString();
                Description = row["Description"].ToString();
        }
    }
}
