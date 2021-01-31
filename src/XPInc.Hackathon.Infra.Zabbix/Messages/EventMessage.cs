using System;
using Newtonsoft.Json;
using XPInc.Hackathon.Infra.Zabbix.Attributes;
using XPInc.Hackathon.Infra.Zabbix.Enums;
using XPInc.Hackathon.Infra.Zabbix.Extensions;

namespace XPInc.Hackathon.Infra.Zabbix.Messages
{
    [Method(Types.Incidents)]
    public sealed class EventModel
    {
        [JsonProperty("limit")] public int? Limit { get; set; }
        [JsonProperty("groupids")] public int[] GroupIds { get; set; }
        [JsonProperty("acknowledged")] public bool Acknowledged { get; set; }

    }



    public sealed class EventMessage : HttpSendMessageBase<EventModel>
    {
        public EventMessage(int limit)
        {
            this.Message =
                new EventModel
                {
                    Limit = limit
                };
            this.Authentication();
        }

        public EventMessage(int[] groupIds, bool acknowledged)
        {
            this.Message =
                new EventModel
                {
                    Limit = null,
                    GroupIds = groupIds,
                    Acknowledged = acknowledged
                };
            this.Authentication();
        }

        public EventMessage(DateTime dateInit, int[] groupIds, bool acknowledged)
        {
            this.Message =
                new EventModel
                {
                    Limit = null,
                    GroupIds = groupIds,
                    Acknowledged = acknowledged

                };
            this.Authentication();
        }



    }
}
