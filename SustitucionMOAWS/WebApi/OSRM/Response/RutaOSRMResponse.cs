using SustitucionMOAWS.WebApi.OSRM.Common;

namespace SustitucionMOAWS.WebApi.OSRM.Response
{
    public class RutaOSRMResponse
    {
        public RouteResponse<GeoJsonGeometry> RouteResponse { get; set; }

        public string JsonResponseRaw { get; set; }
    }
}
