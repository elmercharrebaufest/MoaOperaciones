using Newtonsoft.Json;
using SustitucionMOAWS.WebApi.OSRM.Common;
using SustitucionMOAWS.WebApi.OSRM.Response.Common;

namespace SustitucionMOAWS.WebApi.OSRM.Response
{
    public class RouteResponse<TGeometry> : CommonResponse where TGeometry : Geometry
    {
        /// <summary>
        /// Array of Waypoint objects sorted by distance to the input coordinate.
        /// </summary>
        [JsonProperty("waypoints")]
        public Waypoint[] Waypoints { get; set; }

        /// <summary>
        /// An array of Route objects, ordered by descending recommendation rank.
        /// </summary>
        [JsonProperty("routes")]
        public Route<TGeometry>[] Routes { get; set; }
    }
}
