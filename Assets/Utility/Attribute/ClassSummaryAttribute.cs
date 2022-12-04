using System;
using UnityEngine;

namespace Utility
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ClassSummaryAttribute : Attribute
    {
        public string Sammary { get; private set; }
        public bool IsFormated { get; private set; } = false;

        public void Format(Component component, string path)
        {
            var dirname = path.Split('\\')[^2];

            Sammary = Sammary
                .Replace("{gameObject.name}", component != null ? component.gameObject.name : "")
                .Replace("{asset.dirname}", dirname);
            IsFormated = true;
        }


        public ClassSummaryAttribute(string sammary)
        {
            Sammary = sammary;
        }
    }
}
