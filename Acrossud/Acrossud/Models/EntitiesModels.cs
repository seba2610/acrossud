using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    public class EntityModel
    {
        public EntityModel()
        {

        }

        public int Id { get; set; }

        [DisplayName("Nombre*")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Características")]
        public List<Property> Properties { get; set; }

        [DisplayName("Documentos")]
        public List<EntityDocumentFileInfo> Documents { get; set; }
    }

    public class EntityDocumentFileInfo
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string ThumbnailFilename { get; set; }
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}