using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// An intersection gives a full representation of any cross-way the path passes bay.
    /// For every step, the very first intersection (intersections[0]) corresponds to the location of the StepManeuver.
    /// Further intersections are listed for every cross-way until the next turn instruction.
    /// </summary>
    public class RouteStepIntersections
    {
        /// <summary>
        /// A [longitude, latitude] pair describing the location of the turn.
        /// </summary>
        [JsonProperty("location")]
        public float[] Location { get; set; }

        /// <summary>
        /// A list of bearing values (e.g. [0,90,180,270]) that are available at the intersection.
        /// The bearings describe all available roads at the intersection.
        /// Values are between 0-359 (0=true north)
        /// </summary>
        [JsonProperty("bearings")]
        public int[] Bearings { get; set; }

        /// <summary>
        /// An array of strings signifying the classes (as specified in the profile) of the road exiting the intersection.
        /// </summary>
        [JsonProperty("classes")]
        public string[] Classes { get; set; }

        /// <summary>
        /// A list of entry flags, corresponding in a 1:1 relationship to the bearings.
        /// A value of true indicates that the respective road could be entered on a valid route.
        /// false indicates that the turn onto the respective road would violate a restriction.
        /// </summary>
        [JsonProperty("entry")]
        public bool[] Entry { get; set; }

        /// <summary>
        /// Index into bearings/entry array.
        /// Used to calculate the bearing just before the turn.
        /// Namely, the clockwise angle from true north to the direction of travel immediately before the maneuver/passing the intersection.
        /// Bearings are given relative to the intersection.
        /// To get the bearing in the direction of driving, the bearing has to be rotated by a value of 180.
        /// The value is not supplied for depart maneuvers.
        /// </summary>
        [JsonProperty("in")]
        public int In { get; set; }

        /// <summary>
        /// Index into the bearings/entry array.
        /// Used to extract the bearing just after the turn.
        /// Namely, The clockwise angle from true north to the direction of travel immediately after the maneuver/passing the intersection.
        /// The value is not supplied for arrive maneuvers.
        /// </summary>
        [JsonProperty("out")]
        public int Out { get; set; }

        /// <summary>
        /// Array of Lane objects that denote the available turn lanes at the intersection.
        /// If no lane information is available for an intersection, the lanes property will not be present.
        /// </summary>
        [JsonProperty("lanes")]
        public RouteStepIntersectionLane[] Lanes { get; set; }
    }
}
