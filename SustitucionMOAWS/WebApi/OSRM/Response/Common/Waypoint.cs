using Newtonsoft.Json;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    public class Waypoint
    {
        /// <summary>
        /// Array of OpenStreetMap node ids.
        /// </summary>
        [JsonProperty("nodes")]
        public long[] Nodes { get; set; }

        /// <summary>
        /// Name of the street the coordinate snapped to.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Array that contains the [longitude, latitude] pair of the snapped coordinate.
        /// </summary>
        [JsonProperty("location")]
        public double[] Location { get; set; }

        /// <summary>
        /// The distance, in metres, from the input coordinate to the snapped coordinate.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// Unique internal identifier of the segment (ephemeral, not constant over data updates).
        /// This can be used on subsequent request to significantly speed up the query and to connect multiple services.
        /// E.g. you can use the hint value obtained by the nearest query as hint values for route inputs.
        /// </summary>
        [JsonProperty("hint")]
        public string Hint { get; set; }
    }
}
