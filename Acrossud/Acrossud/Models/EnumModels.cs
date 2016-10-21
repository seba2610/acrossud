using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Acrossud.Models
{
    public class CurrencyEnumModels
    {
        public IEnumerable<SelectListItem> Items;
        public EnumConst.CurrencyEnum Selected;

        public CurrencyEnumModels()
        {
            var names = Enum.GetNames(typeof(EnumConst.CurrencyEnum));
            var values = Enum.GetValues(typeof(EnumConst.CurrencyEnum)).Cast<EnumConst.CurrencyEnum>();

            Items = names.Zip(values, (name, value) =>
                new SelectListItem { Text = name, Value = value.ToString() });
        }
    }

    public class RentSaleEnumModels
    {
        public IEnumerable<SelectListItem> Items;
        public SelectListItem Selected;
    }
}