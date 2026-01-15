using System.ComponentModel;

namespace FleetlyMobile.Extensions
{
    public static class EnumExtension
    {
        public static string ToDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}
