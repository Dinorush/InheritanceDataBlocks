using System;
using System.Reflection;

namespace InheritanceDataBlocks.Utils
{
    public static class PropertyUtil
    {
        public static PropertyInfo? ToProperty(this string name, Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            PropertyInfo? propertyInfo = type.GetProperty(name, flags);
            if (propertyInfo == null || !propertyInfo.CanWrite)
                return null;
            return propertyInfo;
        }

        // Ripped from InjectLib
        public static void CopyProperties<T>(T source, T target)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var propertyType = prop.PropertyType;

                if (prop.Name.Contains("_k__BackingField"))
                    continue;

                if (propertyType == typeof(IntPtr))
                    continue;

                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                prop.SetValue(target, prop.GetValue(source));
            }
        }
    }
}
