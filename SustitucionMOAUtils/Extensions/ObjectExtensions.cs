using Newtonsoft.Json;

namespace SustitucionMOAUtils.Extensions
{
	public static class ObjectExtensions
	{
		public static string GetStringJson<T>(this T t) where T : class
		{
			return JsonConvert.SerializeObject(t);
		}
	}
}
