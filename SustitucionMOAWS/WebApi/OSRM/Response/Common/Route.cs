using Newtonsoft.Json;
using SustitucionMOAWS.WebApi.OSRM.Common;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// Represents a route through (potentially multiple) waypoints.
    /// </summary>
    /// <typeparam name="TGeometry"></typeparam>
    public class Route<TGeometry> where TGeometry : Geometry
    {
        /// <summary>
        /// The distance traveled by the route, in float meters.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// The estimated travel time, in float number of seconds.
        /// </summary>
        [JsonProperty("duration")]
        public float Duration { get; set; }

        /// <summary>
        /// The whole geometry of the route value depending on overview parameter, format depending on the geometries parameter.
        /// See RouteStep's geometry property for a parameter documentation.
        /// <seealso cref="RouteStep.Geometry"/>
        /// </summary>
        [JsonProperty("geometry")]
        public TGeometry Geometry { get; set; }

        /// <summary>
        /// The calculated weight of the route.
        /// </summary>
        [JsonProperty("weight")]
        public float Weight { get; set; }

        /// <summary>
        /// The name of the weight profile used during extraction phase.
        /// </summary>
        [JsonProperty("weight_name")]
        public string WeightName { get; set; }

        /// <summary>
        /// The legs between the given waypoints, an array of RouteLeg objects.
        /// </summary>
        [JsonProperty("legs")]
        public RouteLeg<TGeometry>[] Legs { get; set; }
    }
}
