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
        public int Order { get; set; }

        public Property()
        {

        }

        public Property(DataRow row)
        {
            Id = Int32.Parse(row["Id"].ToString());
            Name = row["Name"].ToString();
            Description = row["Description"] == DBNull.Value ? String.Empty: row["Description"].ToString();
            Type = Convert(row["Type"].ToString());
            Order = Int32.Parse(row["Order"].ToString());

            bool contains_value = false;

            try
            {
                if (row["Value"] != null)
                    contains_value = true;
            }
            catch
            {
                contains_value = false;
            }

            switch (Type)
            {
                case PropertyType.Int:
                    if (contains_value)
                        Value = Int32.Parse(row["Value"].ToString());
                    else
                        Value = 0;
                    break;
                case PropertyType.String:
                    if (contains_value)
                        Value  =row["Value"].ToString();
                    else
                        Value = String.Empty;
                    break;
                case PropertyType.Bool:
                    if (contains_value)
                        Value = Boolean.Parse(row["Value"].ToString());
                    else
                        Value = false;
                    break;
                case PropertyType.RentSaleEnum:
                    if (contains_value)
                        Value = (RentSaleEnum)Enum.Parse(typeof(RentSaleEnum), row["Value"].ToString());
                    else
                        Value = RentSaleEnum.Alquiler;
                    break;
                case PropertyType.CurrencyEnum:
                    if (contains_value)
                        Value = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), row["Value"].ToString());
                    else
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
