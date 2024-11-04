using Newtonsoft.Json;
using SustitucionMOAWS.WebApi.OSRM.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// A step consists of a maneuver such as a turn or merge, followed by a distance of travel along a single way to the subsequent step.
    /// </summary>
    public class RouteStep<TGeometry> where TGeometry : Geometry
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
        /// The unsimplified geometry of the route segment, depending on the geometries parameter.
        /// </summary>
        [JsonProperty("geometry")]
        public TGeometry Geometry { get; set; }

        /// <summary>
        /// The calculated weight of the step.
        /// </summary>
        [JsonProperty("weight")]
        public float Weight { get; set; }

        /// <summary>
        /// The name of the way along which travel proceeds.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A reference number or code for the way. Optionally included, if ref data is available for the given way.
        /// </summary>
        [JsonProperty("ref")]
        public string Ref { get; set; }

        /// <summary>
        /// A string containing an IPA phonetic transcription indicating how to pronounce the name in the name property.
        /// This property is omitted if pronunciation data is unavailable for the step.
        /// </summary>
        [JsonProperty("pronunciation")]
        public string Pronunciation { get; set; }

        /// <summary>
        /// The destinations of the way. Will be undefined if there are no destinations.
        /// </summary>
        [JsonProperty("destinations")]
        public string Destinations { get; set; } = "undefined";

        /// <summary>
        /// The exit numbers or names of the way. Will be undefined if there are no exit numbers or names.
        /// </summary>
        [JsonProperty("exits")]
        public string Exits { get; set; } = "undefined";

        /// <summary>
        /// A string signifying the mode of transportation.
        /// </summary>
        [JsonProperty("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// A StepManeuver object representing the maneuver.
        /// </summary>
        [JsonProperty("maneuver")]
        public RouteStepManeuver Maneuver { get; set; }

        /// <summary>
        /// A list of Intersection objects that are passed along the segment, the very first belonging to the StepManeuver.
        /// </summary>
        [JsonProperty("intersections")]
        public RouteStepIntersections[] Intersections { get; set; }

        /// <summary>
        /// The name for the rotary. Optionally included, if the step is a rotary and a rotary name is available.
        /// </summary>
        [JsonProperty("rotary_name")]
        public string RotaryName { get; set; }

        /// <summary>
        /// The pronunciation hint of the rotary name. Optionally included, if the step is a rotary and a rotary pronunciation is available.
        /// </summary>
        [JsonProperty("rotary_pronunciation")]
        public string RotaryPronunciation { get; set; }

        /// <summary>
        /// The legal driving side at the location for this step. Either left or right.
        /// </summary>
        [JsonProperty("driving_side")]
        public string DrivingSide { get; set; }
    }
}
