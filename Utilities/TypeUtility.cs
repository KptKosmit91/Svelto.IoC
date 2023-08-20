using System;

namespace Svelto.IoC
{
    public static class TypeUtility
    {
        public static bool InheritsType(this Type baseType, Type implementationType)
        {
            return implementationType.IsAssignableFrom(baseType);
        }
    }
}
