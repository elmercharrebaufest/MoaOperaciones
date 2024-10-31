using System;

namespace SustitucionMOAModel.Attributes
{
    /// <summary>
    /// Definir tamaño para las columnas
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnWidthAttribute : Attribute
    {
        public uint Width { get; set; }

        public ExcelColumnWidthAttribute(uint width)
        {
            Width = width;
        }
    }
}
