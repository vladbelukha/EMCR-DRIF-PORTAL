using System.ComponentModel;
using System;
using EMCR.DRR.Managers.Intake;

namespace EMCR.Utilities.Extensions
{
    public static class IEnumEx
    {
        public static string ToDescriptionString<T>(this T val) where T : Enum
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603 // Possible null reference return.
                }
            }

            //throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
#pragma warning disable CS8603 // Possible null reference return.
            return default(T);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
