using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoLedgeBook.ViewModels;

public abstract class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged = (_,__)=> { };

    protected void InvokePropertyChanged([CallerMemberName] string memberName = "")
        => PropertyChanged(this, new PropertyChangedEventArgs(memberName));
    
    protected void ChangeProperty<T>(ref T property, T value, [CallerMemberName] string memberName = "")
    {
        property = value;
        InvokePropertyChanged(memberName);
    }

    private void GetLinkedProperties()
    {
        Type currentType = GetType();
        PropertyInfo[] typeProperties = currentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Dictionary<LinkedPropertyAttribute, PropertyInfo> linkAttributeToProperty = new Dictionary<LinkedPropertyAttribute, PropertyInfo>();

        foreach (PropertyInfo property in typeProperties)
        {
            LinkedPropertyAttribute? linkAttribute = property.GetCustomAttribute<LinkedPropertyAttribute>();
            if (linkAttribute is null)
                continue;
            linkAttributeToProperty[linkAttribute] = property;
        }


    }
}

[System.AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class LinkedPropertyAttribute : System.Attribute
{ 
    public LinkedPropertyAttribute(params string[] linkedProperties)
    {
        LinkedProperties = linkedProperties;
    }

    public string[] LinkedProperties { get; init; }
}
