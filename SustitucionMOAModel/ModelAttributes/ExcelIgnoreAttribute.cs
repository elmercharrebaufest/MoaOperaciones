using System;

namespace SustitucionMOAModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelIgnoreAttribute : Attribute
    {
    }
}
