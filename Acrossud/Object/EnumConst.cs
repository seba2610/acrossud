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

        public const string EntityImagesPath = @"{0}images\\{1}";

        public const string PropertyNameMainPicture = "Imagen principal";
    }
}
