using Newtonsoft.Json;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// A Lane represents a turn lane at the corresponding turn location.
    /// </summary>
    public class RouteStepIntersectionLane
    {
        /// <summary>
        /// An indication (e.g. marking on the road) specifying the turn lane.
        /// A road can have multiple indications (e.g. an arrow pointing straight and left).
        /// The indications are given in an array, each containing one of the following types.
        /// Further indications might be added on without an API version change.
        /// </summary>
        [JsonProperty("indications")]
        public string[] Indications { get; set; }

        /// <summary>
        /// A boolean flag indicating whether the lane is a valid choice in the current maneuver.
        /// </summary>
        [JsonProperty("valid")]
        public bool Valid { get; set; }
    }
}
