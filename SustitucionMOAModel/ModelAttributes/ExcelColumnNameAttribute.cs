using System;

namespace SustitucionMOAModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ExcelColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
