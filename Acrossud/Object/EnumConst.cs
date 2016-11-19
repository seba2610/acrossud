using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrossud
{
    public class EnumConst
    {
        public enum DataAccessProvider { SqlServer }

        public enum PropertyType { Int, String, Bool, RentSaleEnum, CurrencyEnum, Unkown }

        public enum RentSaleEnum { Alquiler, Venta}

        public enum CurrencyEnum { Dólares, Pesos }

        public enum PropertyValue { True, False }

        public enum PropertyOperator { Equal, Greater, GreaterEqual, Less, LessEqual, Contains }

        public const string EntityImagesPath = @"{0}/images/{1}";

        public const string NoImageFile = @"{0}/images/no_image.png";

        public const string EntityThumbnailImagesPath = @"{0}/thumbnail/";

        public const string AbsoluteFileName = "{0}/{1}";

        public const string ThumbnailDir = "thumbnail";

        public const int ThumbnailDefaultSize = 120;

        public const int ImageDefaultWidth = 120;

        public const int ImageDefaultHeight = 120;

        public const string PropertyNameMainPicture = "Imagen principal";

        public const string PropertyNameRentSale = "Venta o alquiler";

        public const string PropertyNamePrice = "Precio";

        public const string PropertyNamePriceGc = "Gastos comunes";

        public const string PropertyNameCurrency = "Tipo de moneda";

        public const string PropertyNameCurrencyGc = "Tipo de moneda (Gastos comunes)";

        public const string PropertyNameFeatured = "Destacada";

        public const string PropertyNameActive = "Activa";

        public const string PropertyValueTrue = "Sí";

        public const string PropertyValueFalse = "No";
    }
}
