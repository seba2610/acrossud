using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Acrossud.EnumConst;

namespace Acrossud
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
        public PropertyType Type { get; set; }

        public Property()
        {

        }

        public Property(DataRow row)
        {
            Id = Int32.Parse(row["Id"].ToString());
            Name = row["Name"].ToString();
            Description = row["Description"] == DBNull.Value ? String.Empty: row["Description"].ToString();
            Type = Convert(row["Type"].ToString());

            switch (Type)
            {
                case PropertyType.Int:
                    Value = 0;
                    break;
                case PropertyType.String:
                    Value = String.Empty;
                    break;
                case PropertyType.Bool:
                    Value = false;
                    break;
                case PropertyType.RentSaleEnum:
                    Value = RentSaleEnum.Alquiler;
                    break;
                case PropertyType.CurrencyEnum:
                    Value = CurrencyEnum.Pesos;
                    break;
            }
        }

        public static PropertyType Convert(string type)
        {
            switch (type)
            {
                case "Int": return PropertyType.Int;
                case "String": return PropertyType.String;
                case "Bool": return PropertyType.Bool;
                case "RentSaleEnum": return PropertyType.RentSaleEnum;
                case "CurrencyEnum": return PropertyType.CurrencyEnum;
                default: return PropertyType.Unkown;
            }
        }
    }
}
