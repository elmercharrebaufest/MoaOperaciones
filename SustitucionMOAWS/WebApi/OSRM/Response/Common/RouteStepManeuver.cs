using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    public class RouteStepManeuver
    {
        /// <summary>
        /// A [longitude, latitude] pair describing the location of the turn.
        /// </summary>
        [JsonProperty("location")]
        public double[] Location { get; set; }

        /// <summary>
        /// The clockwise angle from true north to the direction of travel immediately before the maneuver. Range 0-359.
        /// </summary>
        [JsonProperty("bearing_before")]
        public int BearingBefore { get; set; }

        /// <summary>
        /// The clockwise angle from true north to the direction of travel immediately after the maneuver. Range 0-359.
        /// </summary>
        [JsonProperty("bearing_after")]
        public int BearingAfter { get; set; }

        /// <summary>
        /// A string indicating the type of maneuver. 
        /// New identifiers might be introduced without API change Types unknown to the client should be handled like the turn type, the existence of correct modifier values is guranteed.
        /// Please note that even though there are new name and notification instructions, the mode and name can change between all instructions.
        /// They only offer a fallback in case nothing else is to report.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// An optional string indicating the direction change of the maneuver.
        /// </summary>
        [JsonProperty("modifier")]
        public string Modifier { get; set; }

        /// <summary>
        /// An optional integer indicating number of the exit to take.
        /// The property exists for the roundabout / rotary property: Number of the roundabout exit to take.
        /// If exit is undefined the destination is on the roundabout.
        /// </summary>
        [JsonProperty("exit")]
        public int Exit { get; set; }
    }
}
