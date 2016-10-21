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
        public NewEntityModel()
        {

        }

        public int Id { get; set; }

        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Características")]
        public List<Property> Properties { get; set; }
    }

    public class EntityPictureFileInfo
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public long FileSize { get; set; }
    }
}