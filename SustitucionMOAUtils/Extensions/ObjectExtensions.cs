using Newtonsoft.Json;
using System;

namespace SustitucionMOAUtils.Extensions
{
	public static class ObjectExtensions
	{
		public static string GetStringJson<T>(this T t) where T : class
		{
			var result = JsonConvert.SerializeObject(t, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
			return result;
		}
	}
}
