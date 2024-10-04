using System;

namespace SustitucionMOAModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnTypeAttribute : Attribute
    {
        public ExcelCellType Type { get; set; }

        public ExcelColumnTypeAttribute(ExcelCellType type)
        {
            Type = type;
        }
    }

    public enum ExcelCellType
    {
        Text,
        Number,
        Formula,
    }
}
