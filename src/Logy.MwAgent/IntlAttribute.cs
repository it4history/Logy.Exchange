using System;

namespace Logy.MwAgent
{
    /// <summary>
    /// if applied to assembly then this.Name is wiki base url
    /// if applied to class, enum then this.Name is translation message key prefix
    /// if applied to property then this.Name is translation message key suffix
    /// if applied to enum then enum int values are translation message key suffixes
    /// </summary>
    public class IntlAttribute : Attribute
    {
        public IntlAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}