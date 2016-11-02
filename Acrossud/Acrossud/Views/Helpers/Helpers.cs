using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Acrossud.Views.Helpers
{
    public class Helpers
    {
        public static string GetEntityPrice(Property price, Property currency)
        {
            if (price != null && !String.IsNullOrEmpty(price.Value.ToString()) &&
                currency != null && !String.IsNullOrEmpty(currency.Value.ToString()))
            {
                if ((EnumConst.CurrencyEnum)currency.Value == EnumConst.CurrencyEnum.Dólares)
                    return string.Format("USD {0}", String.Format("{0:0,0}", ((int)price.Value)));
                else
                    return string.Format("$ {0}", String.Format("{0:0,0}", ((int)price.Value)));
            }

            return String.Empty;
        }

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

        public static string GetPropertyValue(Property property)
        {
            string result = String.Empty;

            switch (property.Type)
            {
                case EnumConst.PropertyType.Bool:
                    if ((bool)property.Value)
                        result = EnumConst.PropertyValueTrue;
                    else
                        result = EnumConst.PropertyValueFalse;
                    break;
                default:
                    result = property.Value.ToString();
                    break;
            }

            return result;
        }
    }
}