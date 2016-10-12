using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Acrossud.EnumConst;

namespace Acrossud.Models
{
    public class EntityCollectionModel
    {
        public IEnumerable<Entity> Entities { get; set; }
    }

    public class NewEntityModel
    {
        private List<int> _roomValues;

        public NewEntityModel()
        {
            _roomValues = new List<int>(Enumerable.Range(0, 10));
        }

        public int Id { get; set; }

        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Características")]
        public IEnumerable<Property> Properties { get; set; }

        public IEnumerable<SelectListItem> RoomValues
        {
            get { return new SelectList(_roomValues); }
        }

        public int RoomCountSelected { get; set; }
    }
}