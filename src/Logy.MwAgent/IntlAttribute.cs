using System;

namespace Logy.MwAgent
{
    /// <summary>
    /// if applied to assembly then Name is wiki base url
    /// if applied to class then Name is translation message key prefix
    /// if applied to property then Name is translation message key suffix
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