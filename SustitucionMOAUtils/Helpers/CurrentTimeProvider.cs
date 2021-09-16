using System;
using SustitucionMOAUtils.Interfaces.Helpers;

namespace SustitucionMOAUtils.Helpers
{
    public class CurrentTimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
