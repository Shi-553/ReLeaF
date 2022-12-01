using System;
using System.Reflection;
using UnityEditor;

namespace Utility
{
    public static class SerializedPropertyExtension
    {
        public static T GetAttributes<T>(this SerializedProperty prop, bool inherit) where T : Attribute
        {
            if (prop == null) { return null; }

            Type t = prop.serializedObject.targetObject.GetType();

            FieldInfo f = null;
            PropertyInfo p = null;
            foreach (var name in prop.propertyPath.Split('.'))
            {
                f = t.GetField(name, (BindingFlags)(-1));

                if (f == null)
                {
                    p = t.GetProperty(name, (BindingFlags)(-1));
                    if (p == null)
                    {
                        return null;
                    }
                    t = p.PropertyType;
                }
                else
                {
                    t = f.FieldType;
                }
            }

            T[] attributes;

            if (f != null)
            {
                attributes = f.GetCustomAttributes(typeof(T), inherit) as T[];
            }
            else if (p != null)
            {
                attributes = p.GetCustomAttributes(typeof(T), inherit) as T[];
            }
            else
            {
                return null;
            }
            return attributes.Length > 0 ? attributes[0] : null;
        }



        // https://gist.github.com/SixWays/2257d0c129c5cb7db96c7990af823b44

        const BindingFlags FLAGS_ALL = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Get the object this property
        /// </summary>
        public static object GetValue(this SerializedProperty p)
        {
            object o = p.serializedObject.targetObject;
            // Friendly array syntax - one . per path element
            // e.g. propName.Array.data[0] => propName[0]
            string path = p.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');
            for (int i = 0; i < elements.Length; ++i)
            {
                if (p.IsArrayElement(elements[i]))
                {
                    string arrayName;
                    int j = GetArrayIndex(elements, i, out arrayName);
                    var fi = o.GetType().GetFieldPrivate(arrayName, FLAGS_ALL);
                    var arr = (System.Array)fi.GetValue(o);
                    o = arr.GetValue(j);
                }
                else
                {
                    var fi = o.GetType().GetFieldPrivate(elements[i], FLAGS_ALL);
                    o = fi.GetValue(o);
                }
            }
            return o;
        }
        public static bool IsArrayElement(this SerializedProperty p)
        {
            return p.propertyPath.EndsWith("]");
        }
        public static bool IsArrayElement(this SerializedProperty p, string path)
        {
            return path.EndsWith("]");
        }
        public static FieldInfo GetArrayField(this SerializedProperty p)
        {
            return p.GetArrayField(-2);
        }
        public static FieldInfo GetArrayField(this SerializedProperty p, int pathElement)
        {
            // Friendly array syntax - one . per path element
            string path = p.propertyPath.Replace("Array.data[", "[");
            string[] dirs = path.Split('.');
            // If index negative, count from end
            string fieldName = pathElement < 0 ? dirs[dirs.Length - pathElement] : dirs[pathElement];

            var t = p.serializedObject.targetObject.GetType();
            return t.GetFieldPrivate(fieldName, FLAGS_ALL);
        }
        static FieldInfo GetArrayField(this SerializedProperty p, string[] pathElements, int elementIndex)
        {
            // If index negative, count from end
            string fieldName = elementIndex < 0 ? pathElements[pathElements.Length - elementIndex] : pathElements[elementIndex];
            var t = p.serializedObject.targetObject.GetType();
            return t.GetFieldPrivate(fieldName, FLAGS_ALL);
        }
        public static int GetArrayIndex(this SerializedProperty p)
        {
            // Remove cruft
            string path = p.propertyPath.Replace("Array.data[", "");
            path = path.TrimEnd(']');
            string[] dirs = path.Split('.');
            // Final element should be array index
            return int.Parse(dirs[dirs.Length - 1]);
        }
        static int GetArrayIndex(string[] pathElements, int elementIndex, out string arrayName)
        {
            // Find opening array bracket to get first element of int substring
            string e = GetElement(pathElements, elementIndex);
            int first = e.Length - 1;
            for (; first >= 0; --first)
            {
                if (e[first] == '[')
                {
                    ++first;
                    break;
                }
            }
            // Name is substring from [0] to [first-1] (remove [{index}])
            arrayName = e.Substring(0, first - 1);
            // Get substring from [first] to [penultimate element] (get int within [])
            int len = e.Length - (first + 1);
            return int.Parse(e.Substring(first, len));
        }
        static string GetElement(string[] elements, int index)
        {
            return index < 0 ? elements[elements.Length - index] : elements[index];
        }

        public static FieldInfo GetFieldPrivate(this Type t, string fieldName, BindingFlags flags)
        {
            flags |= BindingFlags.NonPublic;
            FieldInfo result = null;
            Type type = t;
            Type bt = typeof(object);
            while (type != bt)
            {
                result = type.GetField(fieldName, flags);
                if (result != null) break;
                type = type.BaseType;
            }
            return result;
        }
        public static PropertyInfo GetPropertyPrivate(this Type t, string propName, BindingFlags flags)
        {
            flags |= BindingFlags.NonPublic;
            PropertyInfo result = null;
            Type type = t;
            Type bt = typeof(object);
            while (type != bt)
            {
                result = type.GetProperty(propName, flags);
                if (result != null) break;
                type = type.BaseType;
            }
            return result;
        }
        public static MethodInfo GetMethodPrivate(this Type t, string methodName, BindingFlags flags)
        {
            flags |= BindingFlags.NonPublic;
            MethodInfo result = null;
            Type type = t;
            Type bt = typeof(object);
            while (type != bt)
            {
                result = type.GetMethod(methodName, flags);
                if (result != null) break;
                type = type.BaseType;
            }
            return result;
        }
    }
}