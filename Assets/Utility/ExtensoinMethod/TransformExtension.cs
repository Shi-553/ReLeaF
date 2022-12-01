using System.Collections.Generic;
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
        public static IEnumerable<Transform> GetChildren(this Transform t)
        {
            var children = new Transform[t.childCount];

            for (int i = 0; i < t.childCount; i++)
            {
                children[i] = t.GetChild(i);
            }
            return children;
        }
    }
}
