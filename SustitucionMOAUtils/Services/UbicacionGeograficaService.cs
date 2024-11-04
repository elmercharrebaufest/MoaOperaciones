using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WebApi.OpenStreetMap.Response;
using SustitucionMOAWS.WebApi.OSRM.Common;
using SustitucionMOAWS.WebApi.OSRM.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class UbicacionGeograficaService : IUbicacionGeograficaService
    {
        private readonly IRepositorioUbicacionGeografica repositorioUbicacionGeo;
        private readonly IOsrmApiClient osrmApiClient;
        private readonly IOpenStreetMapClient openStreetMapClient;
        private readonly IEmailUbicacionGeograficaService emailUbicacionGeograficaService;

        public UbicacionGeograficaService(
            IRepositorioUbicacionGeografica repositorioUbicacionGeo,
            IOsrmApiClient osrmApiClient,
            IOpenStreetMapClient openStreetMapClient,
            IEmailUbicacionGeograficaService emailUbicacionGeograficaService)
        {
            this.repositorioUbicacionGeo = repositorioUbicacionGeo;
            this.osrmApiClient = osrmApiClient;
            this.openStreetMapClient = openStreetMapClient;
            this.emailUbicacionGeograficaService = emailUbicacionGeograficaService;
        }


        public DistanciaDomicilio ObtenerDistanciaDePlantaMoaADestino(string direccionDestino) //public async Task<DistanciaDomicilio> ObtenerDistanciaDePlantaMoaADestino(string direccionDestino)
        {
            direccionDestino = direccionDestino ?? throw new ArgumentNullException(nameof(direccionDestino));

            var distanciaDomicilio =
                repositorioUbicacionGeo.ObtenerDistanciaSegunDescripcionDomicilio(direccionDestino) ??
                GenerarNuevaDistanciaDesdeMoa(direccionDestino);

            if (distanciaDomicilio == null || distanciaDomicilio.DistanciaKm == null || distanciaDomicilio.DistanciaKm == 0)
            {
                emailUbicacionGeograficaService.EnviarMailDistanciaNoEncontrada(direccionDestino, distanciaDomicilio?.DireccionBuscada ?? "");
                Log.Info($"No se encontró distancia para el domicilio seleccionado. La dirección ingresada fue {direccionDestino}. La búsqueda se realizó con '{distanciaDomicilio?.DireccionBuscada}'.");
            }
            return distanciaDomicilio;
        }

        private DistanciaDomicilio GenerarNuevaDistanciaDesdeMoa(string direccionDestino) //private async Task<DistanciaDomicilio> GenerarNuevaDistanciaDesdeMoa(string direccionDestino)
        {
            var distanciaDomicilio = new DistanciaDomicilio { DomicilioDescripcion = direccionDestino };

            var lugarDestino = ObtenerLugarDestinoAsync(direccionDestino, distanciaDomicilio); //var lugarDestino = await ObtenerLugarDestinoAsync(direccionDestino, distanciaDomicilio);

            if (lugarDestino != null)
            {
                var rutaDesdeMoa = ObtenerRutaDesdePlantaMOA(lugarDestino, distanciaDomicilio); //var rutaResponse = await ObtenerRutaDesdePlantaMOA(lugarDestino);
                if (rutaDesdeMoa != null)
                {
                    distanciaDomicilio.DistanciaKm = (int)Math.Round((rutaDesdeMoa.Distance / 1000), MidpointRounding.AwayFromZero);
                }
            }
            repositorioUbicacionGeo.Agregar(distanciaDomicilio);
            repositorioUbicacionGeo.GuardarCambios();
            return distanciaDomicilio;
        }

        private OSMPlace ObtenerLugarDestinoAsync(string direccionDestino, DistanciaDomicilio distanciaDomicilio) //private async Task<OSMPlace> ObtenerLugarDestinoAsync(string direccionDestino, DistanciaDomicilio distanciaDomicilio)
        {
            var direccionABuscar = GenerarDireccionABuscar(direccionDestino);
            distanciaDomicilio.DireccionBuscada = direccionABuscar;

            var lugaresResponse = openStreetMapClient.BuscarLugaresSegunDireccionAsync(direccionABuscar).GetAwaiter().GetResult(); //var lugaresResponse = await openStreetMapClient.BuscarLugaresSegunDireccionAsync(direccionABuscar);

            if (lugaresResponse != null)
            {
                distanciaDomicilio.JsonLugaresOSM = lugaresResponse.JsonResponseRaw;
                if (lugaresResponse.Lugares != null && lugaresResponse.Lugares.Count > 0)
                {
                    return lugaresResponse.Lugares[0];
                }
            }
            return null;
        }

        private string GenerarDireccionABuscar(string direccionDestino)
        {
            var direccionABuscar = direccionDestino;
            var reemplazos = repositorioUbicacionGeo.ObtenerReemplazosParaDomicilios();

            foreach (var reemplazo in reemplazos)
            {
                direccionABuscar = direccionABuscar.Replace(reemplazo.Anterior, reemplazo.Nuevo);
            }

            var partesDireccion = direccionABuscar.Split(',');
            if (partesDireccion.Length < 2)
            {
                direccionABuscar = "";
            }
            else
            {
                var localidadNombre = partesDireccion[partesDireccion.Length - 2].Trim();
                var provinciaNombre = partesDireccion[partesDireccion.Length - 1].Trim();
                direccionABuscar = $"{localidadNombre}, {provinciaNombre}, Argentina";
            }
            return direccionABuscar;
        }

        private Route<GeoJsonGeometry> ObtenerRutaDesdePlantaMOA(OSMPlace lugarDestino, DistanciaDomicilio distanciaDomicilio) //private async Task<RutaOSRMResponse> ObtenerRutaDesdePlantaMOAAsync(OSMPlace lugarDestino)
        {
            var coordenadasPlantaMoa = new GeoCoordenada { Latitud = -32.773465, Longitud = -60.728782 };
            var coordenadasDestino = new GeoCoordenada { Latitud = lugarDestino.Lat, Longitud = lugarDestino.Lon };

            var rutaResponse = osrmApiClient.ObtenerRuta(coordenadasPlantaMoa, coordenadasDestino); //var rutaResponse = await osrmApiClient.ObtenerRuta(coordenadasPlantaMoa, coordenadasDestino);

            if (rutaResponse != null)
            {
                distanciaDomicilio.JsonRutaOSRM = rutaResponse.JsonResponseRaw;
                if (rutaResponse.RouteResponse?.Routes != null && rutaResponse.RouteResponse.Routes.Count() > 0)
                {
                    var rutaElegida = rutaResponse.RouteResponse.Routes[0];
                    return rutaElegida;
                }
            }
            return null;
        }
    }
}
