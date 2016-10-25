using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Acrossud.Views.Helpers
{
    public class Helpers
    {
        public static string GetEntityPrice(Property price, Property currency, Property rent_sale)
        {
            if (price != null && !String.IsNullOrEmpty(price.Value.ToString()) && 
                currency != null && !String.IsNullOrEmpty(currency.Value.ToString()) &&
                rent_sale != null && !String.IsNullOrEmpty(rent_sale.Value.ToString()))
            {
                if ((EnumConst.CurrencyEnum)currency.Value == EnumConst.CurrencyEnum.Dólares)
                    return string.Format("USD {0} - {1}", String.Format("{0:0,0}", ((int)price.Value)), rent_sale.Value.ToString());
                else
                    return string.Format("$ {0} - {1}", String.Format("{0:0,0}", ((int)price.Value)), rent_sale.Value.ToString());
            }

            return String.Empty;
        }
    }
}