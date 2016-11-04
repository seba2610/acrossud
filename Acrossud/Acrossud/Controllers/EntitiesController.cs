using System;
using System.Collections.Generic;
using System.Drawing;
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
        public string GetBaseUrl()
        {
            string result = string.Format("{0}/", Request.Url.GetLeftPart(UriPartial.Authority));

            return result;
        }

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

        #region Creation
        public ActionResult CreateNew()
        {
            EntityModel model = new EntityModel();
            model.Properties = EntityMger.Instance.GetProperties().Where(p => p.Name != EnumConst.PropertyNameMainPicture).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateNew(EntityModel model)
        {
            Entity entity = new Entity();
            entity.Name = model.Name;
            entity.Description = model.Description == null ? String.Empty : model.Description;
            entity.Id = EntityMger.Instance.CreateEntity(entity);

            if (entity.Id > 0)
            {
                foreach (Property property in model.Properties)
                {
                    EntityProperty ep = new EntityProperty();
                    ep.EntityId = entity.Id;
                    ep.PropertyId = property.Id;
                    ep.Value = property.Value;
                    ep.Id = EntityMger.Instance.AddOrUpdateEntityProperty(ep);
                }

                string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath("/"), entity.Id.ToString())).ToString();

                string thumbnail_path = string.Format(EnumConst.EntityThumbnailImagesPath, path, entity.Id.ToString());

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!Directory.Exists(thumbnail_path))
                    Directory.CreateDirectory(thumbnail_path);

                return RedirectToAction("GetEntityDocument", new {entity_name = entity.Name, entity_id = entity.Id });
            }
            else
                return RedirectToAction("Index");
        }
        #endregion

        #region Edition
        public ActionResult EditEntity(int entity_id)
        {
            Entity entity = EntityMger.Instance.GetEntity(entity_id);

            EntityModel model = new EntityModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };

            model.Properties = EntityMger.Instance.GetValueOfPropertiesByEntityId(entity_id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEntity(EntityModel model)
        {
            Entity entity = new Entity();
            entity.Name = model.Name;
            entity.Description = model.Description == null ? String.Empty : model.Description;
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
            string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath("/"), entity_id.ToString())).ToString();

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            int result = EntityMger.Instance.DeleteEntity(entity_id);

            return RedirectToAction("Index", new { alert_title = "Propiedad eliminada", alert = "Se ha eliminado la propiedad con éxito." });
        }
        #endregion

        #region Documents
        public ActionResult GetEntityDocument(string entity_name, int entity_id)
        {
            ViewBag.EntityName = entity_name;
            ViewBag.EntityId = entity_id;
            return View();
        }

        public ActionResult AddEntityDocument(int entity_id)
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

                        string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath("/"), entity_id.ToString())).ToString();

                        string thumbnail_path = string.Format(EnumConst.EntityThumbnailImagesPath, path, entity_id.ToString());

                        string absolute_file_name = string.Format(EnumConst.AbsoluteFileName, path, file_name);

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        if (!Directory.Exists(thumbnail_path))
                            Directory.CreateDirectory(thumbnail_path);

                        if (Directory.GetFiles(path).Where(f => f != EnumConst.ThumbnailDir).Count() == 0)
                        {
                            Property p = EntityMger.Instance.GetPropertyByName(EnumConst.PropertyNameMainPicture);
                            EntityProperty ep = new EntityProperty();
                            ep.EntityId = entity_id;
                            ep.PropertyId = p.Id;
                            ep.Value = file_name;
                            ep.Id = EntityMger.Instance.AddOrUpdateEntityProperty(ep);
                        }

                        Bitmap bitmap = Utilities.IsImage(file);

                        if (bitmap != null)
                        {
                            int thumb_height = EnumConst.ThumbnailDefaultSize;
                            int thumb_width = EnumConst.ThumbnailDefaultSize;

                            if (bitmap.Height > bitmap.Width)
                            {
                                thumb_width = (int) (bitmap.Width * ((float)thumb_height / (float)bitmap.Height));
                            }
                            else
                            {
                                thumb_height = (int) (bitmap.Height * ((float)thumb_width / (float)bitmap.Width));
                            }

                            Image thumbnail = bitmap.GetThumbnailImage(thumb_width, thumb_height, null, IntPtr.Zero);
                            thumbnail.Save(string.Format(EnumConst.AbsoluteFileName, thumbnail_path, file_name));
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

        public JsonResult GetEntityDocumentsFiles(int entity_id)
        {
            List<EntityDocumentFileInfo> result = new List<EntityDocumentFileInfo>();

            string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath("/"), entity_id.ToString())).ToString();

            if (Directory.Exists(path))
            {
                var folder = new DirectoryInfo(path);

                FileInfo[] files = folder.GetFiles();

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i];
                    string url_file = string.Format(EnumConst.EntityImagesPath, GetBaseUrl(), entity_id);
                    url_file = string.Format(EnumConst.EntityThumbnailImagesPath, url_file);
                    url_file = string.Format(EnumConst.AbsoluteFileName, url_file, file.Name);
                    result.Add(new EntityDocumentFileInfo() { FileName = file.Name, Path = url_file, FileSize = file.Length });
                }
            }

            var jSonSerialize = new JavaScriptSerializer();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void DeleteEntityDocumentFile(int entity_id, string file_name)
        {
            string path = new DirectoryInfo(string.Format(EnumConst.EntityImagesPath, Server.MapPath("/"), entity_id.ToString())).ToString();

            string thumbnail_path = string.Format(EnumConst.EntityThumbnailImagesPath, path, entity_id.ToString());

            string absolute_file_name = string.Format(EnumConst.AbsoluteFileName, path, file_name);

            string absolute_file_name_thumbnail = string.Format(EnumConst.AbsoluteFileName, thumbnail_path, file_name);

            if (System.IO.File.Exists(absolute_file_name)){
                System.IO.File.Delete(absolute_file_name);
            }

            if (System.IO.File.Exists(absolute_file_name_thumbnail))
            {
                System.IO.File.Delete(absolute_file_name_thumbnail);
            }

            if (Directory.GetFiles(path).Where(f => f != EnumConst.ThumbnailDir).Count() == 0)
            {
                int result = EntityMger.Instance.DeleteEntityPropertyByName(entity_id, EnumConst.PropertyNameMainPicture);
            }
        }
        #endregion

    }
}