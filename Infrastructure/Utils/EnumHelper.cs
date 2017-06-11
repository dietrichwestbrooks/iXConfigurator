using System;
using System.ComponentModel;
using System.Linq;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Utils
{
    public static class EnumHelper
    {
        public static T GetValueFromDescription<T>(string description)
            where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var enumType = typeof(T);

            return Enum.GetValues(enumType)
                .OfType<T>()
                .First(enumValue => enumType.GetField(enumValue.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .OfType<DescriptionAttribute>()
                    .Any(enumAttribute => enumAttribute.Description == description));
        }
    }
}
