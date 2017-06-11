using System.ComponentModel;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils
{
    public static class ConverterHelper
    {
        public static TValue Convert<TValue>(object value)
        {
            if (value == null)
                return default(TValue);

            TValue result;

            var valueType = typeof (TValue);

            var converter = TypeDescriptor.GetConverter(valueType);

            if (valueType.IsInstanceOfType(value))
            {
                result = (TValue) value;
            }
            else if (converter.CanConvertFrom(value.GetType()))
            {
                result = (TValue) converter.ConvertFrom(value);
            }
            else
            {
                result = (TValue) System.Convert.ChangeType(value, valueType);
            }

            return result;
        }
    }
}
