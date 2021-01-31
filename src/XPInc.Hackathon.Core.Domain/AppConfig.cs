using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPInc.Hackathon.Core.Domain
{
    public static class AppConfig
    {
        public static string DammedEventsStreamKey => "dammed_events";
        public static string TreatedEventsStreamKey => "treated_events";
        public static string EventsStreamGroupName => "events";

    }
}
