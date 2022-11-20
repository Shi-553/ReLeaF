using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class RenameAttribute : PropertyAttribute
{
    public string NewName { get; private set; }
    public string Tooltip { get; private set; }
    public bool IsFormated { get; private set; } = false;

    public void Format(Component component)
    {
        NewName = FormatImpl(component, NewName);
        Tooltip = FormatImpl(component, Tooltip);
        IsFormated = true;
    }

    string FormatImpl(Component component, string text)
    {
        return text.Replace("{gameObject.name}", component.gameObject.name);
    }

    public RenameAttribute(string name, string tooltip = "")
    {
        NewName = name;
        Tooltip = tooltip;
    }
}