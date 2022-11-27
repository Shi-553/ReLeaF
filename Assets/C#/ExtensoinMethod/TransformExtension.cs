using UnityEngine;

namespace Utility
{
    public static class TransformExtension
    {
        public static string GetFullPath(this Transform t, string separator)
        {
            string path = t.name;
            var parent = t.parent;
            while (parent)
            {
                path = $"{parent.name}{separator}{path}";
                parent = parent.parent;
            }
            return path;
        }
    }
}
