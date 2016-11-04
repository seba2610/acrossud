using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Acrossud.Models;

namespace Acrossud.Controllers
{
    public class HomeController : Controller
    {
        public string GetBaseUrl()
        {
            string result = string.Format("{0}/", Request.Url.GetLeftPart(UriPartial.Authority));

            return result;
        }

        private List<Property> SetPropertiesToShow(List<Property> properties)
        {
            List<Property> result = properties.Where(p =>
                                        p.Name != EnumConst.PropertyNameFeatured &&
                                        p.Name != EnumConst.PropertyNameActive &&
                                        p.Name != EnumConst.PropertyNamePrice &&
                                        p.Name != EnumConst.PropertyNameMainPicture)
                                    .ToList().OrderBy(p => p.Name).ToList();

            Property price_property = properties.FirstOrDefault(p => p.Name == EnumConst.PropertyNamePrice);

            if (price_property != null)
            {
                price_property.Value = Views.Helpers.Helpers.GetEntityPrice(price_property, properties.FirstOrDefault(p => p.Name == EnumConst.PropertyNameCurrency));
            }

            result.Insert(0, price_property);

            return result;
        }

        public ActionResult Index()
        {
            EntityCollectionModel model = new EntityCollectionModel();
            model.Entities = EntityMger.Instance.GetEntitiesFiltered(EnumConst.PropertyNameFeatured, EnumConst.PropertyOperator.Equal, EnumConst.PropertyValue.True);

            foreach (Entity entity in model.Entities)
            {
                entity.Properties = EntityMger.Instance.GetValueOfPropertiesByEntityId(entity.Id);
            }

            return View(model);
        }

        public ActionResult ShowEntity(int entity_id)
        {
            Entity entity = EntityMger.Instance.GetEntity(entity_id);

            EntityModel model = new EntityModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };

            model.Properties = SetPropertiesToShow(EntityMger.Instance.GetValueOfPropertiesByEntityId(entity_id));

            string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath("/"), entity_id.ToString())).ToString();

            model.Documents = new List<EntityDocumentFileInfo>();

            if (Directory.Exists(path))
            {
                List<string> documents = Directory.GetFiles(path).Where(f => f != EnumConst.ThumbnailDir).ToList();

                string url_file = String.Empty;
                string url_file_thumbnail = String.Empty;
                int width = EnumConst.ImageDefaultWidth;
                int height = EnumConst.ImageDefaultHeight;

                foreach (string document in documents)
                {
                    url_file = string.Format(EnumConst.EntityImagesPath, GetBaseUrl(), entity_id);
                    url_file = string.Format(EnumConst.AbsoluteFileName, url_file, Path.GetFileName(document));

                    url_file_thumbnail = string.Format(EnumConst.EntityImagesPath, GetBaseUrl(), entity_id);
                    url_file_thumbnail = string.Format(EnumConst.EntityThumbnailImagesPath, url_file_thumbnail);
                    url_file_thumbnail = string.Format(EnumConst.AbsoluteFileName, url_file_thumbnail, Path.GetFileName(document));

                    try
                    {
                        Bitmap image = (Bitmap)Image.FromFile(document);
                        width = image.Width;
                        height = image.Height;
                    }
                    catch (Exception ex)
                    {

                    }

                    model.Documents.Add(new EntityDocumentFileInfo() { ThumbnailFilename = url_file_thumbnail, FileName = url_file, Height = height, Width = width });
                }
            }

            return View(model);
        }

        public ActionResult GetAllActiveEntities()
        {
            EntityCollectionModel model = new EntityCollectionModel();
            model.Entities = EntityMger.Instance.GetEntitiesFiltered(EnumConst.PropertyNameActive, EnumConst.PropertyOperator.Equal, EnumConst.PropertyValue.True);

            foreach (Entity entity in model.Entities)
            {
                entity.Properties = EntityMger.Instance.GetValueOfPropertiesByEntityId(entity.Id);
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return Redirect("~/#contacto");
        }
    }
}