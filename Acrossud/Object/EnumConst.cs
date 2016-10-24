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

        public const string EntityThumbnailImagesPath = @"{0}/thumbnail/";

        public const string AbsoluteFileName = "{0}/{1}";

        public const string ThumbnailDir = "thumbnail";

        public const int ThumbnailDefaultSize = 120;

        public const string PropertyNameMainPicture = "Imagen principal";

        public const string PropertyNameFeatured = "Destacado";
    }
}
