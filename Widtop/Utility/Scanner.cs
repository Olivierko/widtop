using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Widtop.Utility
{
    public static class Scanner
    {
        private static IEnumerable<Type> GetTypesSafely(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }

        public static IEnumerable<Type> GetSubclassTypesOf<T>()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypesSafely())
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    if (!type.IsSubclassOf(typeof(T)))
                    {
                        continue;
                    }

                    yield return type;
                }
            }
        }
    }
}
