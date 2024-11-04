using System.Collections.Generic;

namespace SustitucionMOAWS.WebApi.OpenStreetMap.Response
{
    public class LugaresOSMResponse
    {
        public List<OSMPlace> Lugares { get; set; }

        public string JsonResponseRaw { get; set; }
    }
}
