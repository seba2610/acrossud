using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Acrossud.Models;
using Newtonsoft.Json;

namespace Acrossud.Controllers
{
    [Authorize]
    public class EntitiesController : Controller
    {
        // GET: Entities
        public ActionResult Index()
        {
            EntityCollectionModel model = new EntityCollectionModel();
            model.Entities = EntityMger.Instance.GetEntities();

            foreach (Entity entity in model.Entities)
            {
                entity.Properties = EntityMger.Instance.GetValueOfPropertiesByEntityId(entity.Id);
            }

            return View(model);
        }

        public ActionResult CreateNew()
        {
            NewEntityModel model = new NewEntityModel();
            model.Properties = EntityMger.Instance.GetProperties();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateNew(NewEntityModel model)
        {
            Entity entity = new Entity();
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Id = EntityMger.Instance.CreateEntity(entity);

            foreach(Property property in model.Properties)
            {
                EntityProperty ep = new EntityProperty();
                ep.EntityId = entity.Id;
                ep.PropertyId = property.Id;
                ep.Value = property.Value;
                ep.Id = EntityMger.Instance.AddOrUpdateEntityProperty(ep);
            }

            if (entity.Id > -1)
                return RedirectToAction("GetEntityPicture", new { entity_id = entity.Id, entity_name = entity.Name });
            else
                return RedirectToAction("Index");
        }

        public ActionResult Edit(int entity_id)
        {
            Entity entity = EntityMger.Instance.GetEntity(entity_id);

            NewEntityModel model = new NewEntityModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };

            model.Properties = EntityMger.Instance.GetValueOfPropertiesByEntityId(entity_id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(NewEntityModel model)
        {
            Entity entity = new Entity();
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Id = model.Id;
            int result = EntityMger.Instance.UpdateEntity(entity);

            foreach (Property property in model.Properties)
            {
                EntityProperty ep = new EntityProperty();
                ep.EntityId = entity.Id;
                ep.PropertyId = property.Id;
                ep.Value = property.Value;
                result = EntityMger.Instance.AddOrUpdateEntityProperty(ep);
            }

            return RedirectToAction("Index", new { alert_title = "Cambios guardados", alert = "Se han guardado los cambios con éxito." });
        }

        public ActionResult Delete(int entity_id)
        {
            int result = EntityMger.Instance.DeleteEntity(entity_id);

            return RedirectToAction("Index", new { alert_title = "Propiedad eliminada", alert = "Se ha eliminado la propiedad con éxito." });
        }

        public ActionResult GetEntityPicture(string entity_name, int entity_id)
        {
            ViewBag.EntityName = entity_name;
            ViewBag.EntityId = entity_id;
            return View();
        }

        public ActionResult AddEntityPicture(int entity_id)
        {
            bool isSavedSuccessfully = true;
            string file_name = String.Empty;
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];

                    if (file != null && file.ContentLength > 0)
                    {
                        file_name = file.FileName;

                        string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath(@"\"), entity_id.ToString())).ToString();

                        string absolute_file_name = string.Format("{0}\\{1}", path, file_name);

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        if (Directory.GetFiles(path).Count() == 0)
                        {
                            Property p = EntityMger.Instance.GetPropertyByName(EnumConst.PropertyNameMainPicture);
                            EntityProperty ep = new EntityProperty();
                            ep.EntityId = entity_id;
                            ep.PropertyId = p.Id;
                            ep.Value = file_name;
                            ep.Id = EntityMger.Instance.AddOrUpdateEntityProperty(ep);
                        }

                        file.SaveAs(absolute_file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
            {
                return Json(new { Message = file_name });
            }
            else
            {
                return Json(new { Message = "Error al guardar el archivo." });
            }
        }

        public JsonResult GetEntityPicturesFiles(int entity_id)
        {
            List<EntityPictureFileInfo> result = new List<EntityPictureFileInfo>();

            string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath(@"\"), entity_id.ToString())).ToString();

            var folder = new DirectoryInfo(path);

            FileInfo[] files = folder.GetFiles();

            for(int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i];
                result.Add(new EntityPictureFileInfo() { FileName = file.Name, Path = path, FileSize = file.Length });
            }

            var jSonSerialize = new JavaScriptSerializer();

            return Json(result);
        }
    }
}