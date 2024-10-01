using System;
using System.Runtime.Caching;

namespace SustitucionMOARepositorio
{
    public class Cache : ICache
    {
        private MemoryCache cache;

        public Cache()
        {
            cache = MemoryCache.Default;
        }

        public TEntidad Agregar<TEntidad>(string clave, TEntidad entidad, DateTimeOffset? tiempoDeExpiracion = null) where TEntidad : class
        {
            cache.Set(clave, entidad, tiempoDeExpiracion ?? DateTimeOffset.Now.AddSeconds(30));

            return entidad;
        }

        public void Dispose()
        {
            cache.Dispose();
        }

        public bool Existe(string clave)
        {
            return cache.Contains(clave);
        }

        public TEntidad Obtener<TEntidad>(string clave) where TEntidad : class
        {
            return (TEntidad)cache.Get(clave);
        }

        public bool IntentarObtener<TEntidad>(string clave, out TEntidad objeto) where TEntidad : class
        {
            objeto = null;
            if (!Existe(clave)) { return false; }
            objeto = Obtener<TEntidad>(clave);
            return true;
        }

        public void Remover(string clave)
        {
            if (Existe(clave)) cache.Remove(clave);
        }
    }
}
