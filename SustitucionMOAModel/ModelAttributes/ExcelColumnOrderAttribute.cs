using System;

namespace SustitucionMOAModel.Attributes
{
    /// <summary>
    /// Definir orden para las columnas.
    /// A menor número, antes aparece.
    /// Si hay empate, se debe decidir de otra forma
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExcelColumnOrderAttribute : Attribute
    {
        public int Order { get; set; }

        public ExcelColumnOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
