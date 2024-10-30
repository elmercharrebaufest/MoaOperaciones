using Newtonsoft.Json;
using SustitucionMOAWS.WebApi.OSRM.Common;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// Represents a route between two waypoints.
    /// </summary>
    public class RouteLeg<TGeometry> where TGeometry : Geometry
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
        /// The calculated weight of the route.
        /// </summary>
        [JsonProperty("weight")]
        public float Weight { get; set; }

        /// <summary>
        /// Summary of the route taken as string. Depends on the summary parameter:
        /// Names of the two major roads used. Can be empty if route is too short. Or empty string.
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Array of RouteStep objects describing the turn-by-turn instructions.
        /// </summary>
        [JsonProperty("steps")]
        public RouteStep<TGeometry>[] Steps { get; set; }

        /// <summary>
        /// Additional details about each coordinate along the route geometry:
        /// </summary>
        [JsonProperty("annotations")]
        public RouteLegAnnotation Annotations { get; set; }
    }
}
