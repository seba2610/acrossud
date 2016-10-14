using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Acrossud.EnumConst;

namespace Acrossud.Models
{
    public class PropertyModelBinder : DefaultModelBinder
    {
        public override object BindModel(
            ControllerContext ControllerContext,
            ModelBindingContext BindingContext)
        {
            if (BindingContext == null)
            {
                throw new ArgumentNullException("BindingContext");
            };

            ValueProviderResult IdResult = BindingContext.ValueProvider.GetValue(BindingContext.ModelName + ".Id");
            ValueProviderResult NameResult = BindingContext.ValueProvider.GetValue(BindingContext.ModelName + ".Name");
            ValueProviderResult DescriptionResult = BindingContext.ValueProvider.GetValue(BindingContext.ModelName + ".Description");
            ValueProviderResult ValueResult = BindingContext.ValueProvider.GetValue(BindingContext.ModelName + ".Value");

            if (ValueResult == null)
            {
                return (null);
            };

            string Value = ValueResult.AttemptedValue;
            int Id = Convert.ToInt32(IdResult.AttemptedValue);
            string Name = NameResult == null ? String.Empty : NameResult.AttemptedValue;
            string Description = DescriptionResult == null ? String.Empty : DescriptionResult.AttemptedValue;

            if (String.IsNullOrEmpty(Value))
            {
                return (null);
            };

            int Int;

            if (int.TryParse(Value, out Int))
            {
                return new Property
                {
                    Value = Convert.ToInt32(Value),
                    Id = Id,
                    Name = Name,
                    Description = Description
                };
            };

            bool bool_val;

            if (bool.TryParse(Value, out bool_val))
            {
                return new Property
                {
                    Value = Convert.ToBoolean(Value),
                    Id = Id,
                    Name = Name,
                    Description = Description
                };
            };

            RentSaleEnum rent_sale_val;

            if (Enum.TryParse(Value, out rent_sale_val))
            {
                return new Property
                {
                    Value = (RentSaleEnum)Enum.Parse(typeof(RentSaleEnum), Value),
                    Id = Id,
                    Name = Name,
                    Description = Description
                };
            };

            CurrencyEnum curr_val;

            if (Enum.TryParse(Value, out curr_val))
            {
                return new Property
                {
                    Value = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), Value),
                    Id = Id,
                    Name = Name,
                    Description = Description
                };
            };

            if (Value.Split(',').Count() == 2)
            {
                return new Property
                {
                    Value = true,
                    Id = Id,
                    Name = Name,
                    Description = Description
                };
            }

            return new Property
            {
                Value = Value,
                Id = Id,
                Name = Name,
                Description = Description
            };
        }
    }
}