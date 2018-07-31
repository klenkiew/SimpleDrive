using System;

namespace Common
{
    public static class Ensure
    {
        public static T NotNull<T>(T @object, string name) where T : class
        {
            return @object ?? throw new ArgumentNullException(name);
        }
    }
}