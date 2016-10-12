using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Acrossud.Models;

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
            return View(model);
        }

        public ActionResult AddNew()
        {
            NewEntityModel model = new NewEntityModel();
            model.Properties = EntityMger.Instance.GetProperties();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNew(NewEntityModel model)
        {
            return View();
        }
    }
}