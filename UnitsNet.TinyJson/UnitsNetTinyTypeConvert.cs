using System.ComponentModel;
using System.Globalization;

namespace UnitsNet.TinyJson
{
    public class UnitsNetTinyTypeConvert<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string) == sourceType)
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

            var strVal = value as String;
            if (String.IsNullOrEmpty(strVal))
            {
                return null;
            }

            var qntyVal = strVal.ToString().Split("|");
            var quantity = double.Parse(qntyVal[0]);
            var unitType = qntyVal[1];

            var methods = typeof(T).GetMethods().Where(m => m.Name == "ParseUnit");

            Enum? unitEnum = methods.First().Invoke(null, new[] { unitType }) as Enum;

            return Quantity.From(quantity, unitEnum);
        }

    }
}
