using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReLeaf
{
    public static class TransformExtension
    {
        public static string GetFullPath(this Transform t,string separator)
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
