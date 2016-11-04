using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Acrossud.Views.Helpers
{
    public static class Helpers
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

        public static MvcHtmlString MenuLink(
            this HtmlHelper htmlHelper,
            string linkText,
            string actionName,
            string controllerName)
        {
            string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
            if (actionName == currentAction && controllerName == currentController)
            {
                return htmlHelper.ActionLink(
                    linkText,
                    actionName,
                    controllerName,
                    null,
                    new
                    {
                        @class = "active"
                    });
            }
            return htmlHelper.ActionLink(linkText, actionName, controllerName);
        }
    }


}