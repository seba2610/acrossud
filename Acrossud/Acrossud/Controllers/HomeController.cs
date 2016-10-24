using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Acrossud.Models;

namespace Acrossud.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}