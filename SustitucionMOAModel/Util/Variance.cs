using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Util
{
	public class Variance
	{
		public string PropertyName { get; set; }
		public object ValorAnterior { get; set; }
		public object ValorNuevo { get; set; }
	}

	public static class Comparision
	{
		private const string Valor_SI = "Sí";
		private const string Valor_NO = "No";

        public static List<Variance> Compare<T>(this T val1, T val2)
		{
			var variances = new List<Variance>();
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var v = new Variance
				{
					PropertyName = property.Name,
					ValorAnterior = property.ObtenerValor(val1),
					ValorNuevo = property.ObtenerValor(val2),
                };
				if (v.ValorAnterior == null && v.ValorNuevo == null)
				{
					continue;
				}
				if (
					(v.ValorAnterior == null && v.ValorNuevo != null)
					||
					(v.ValorAnterior != null && v.ValorNuevo == null)
				)
				{
					variances.Add(v);
					continue;
				}
				if (!v.ValorAnterior.Equals(v.ValorNuevo))
				{
					variances.Add(v);
				}
			}
			return variances;
		}

		private static object ObtenerValor<T>(this PropertyInfo propertyInfo, T objeto)
		{
			if (propertyInfo.PropertyType == typeof(bool))
			{
				return (bool)propertyInfo.GetValue(objeto) ? Valor_SI : Valor_NO;
            }
            
			if (propertyInfo.PropertyType == typeof(bool?))
            {
				var valorBoolNulleable = (bool?)propertyInfo.GetValue(objeto);
                if (!valorBoolNulleable.HasValue)
				{
					return "";
				}
				return valorBoolNulleable.Value ? Valor_SI : Valor_NO;
            }

            return propertyInfo.GetValue(objeto);
		}
	}
}
