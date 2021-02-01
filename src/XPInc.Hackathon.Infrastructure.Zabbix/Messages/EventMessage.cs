using Newtonsoft.Json;
using XPInc.Hackathon.Infrastructure.Zabbix.Enums;
using XPInc.Hackathon.Infrastructure.Zabbix.Filters;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Attributes;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Enums;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Extensions;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages
{
    public interface IEventMessage { }


    [Method(TypesEnum.Events)]
    public sealed class EventModel
    {
        [JsonProperty("limit")] public int? Limit { get; set; }
        [JsonProperty("groupids")] public int[] GroupIds { get; set; }
        [JsonProperty("acknowledged")] public bool Acknowledged { get; set; }
        [JsonProperty("sortfield")] public string[] Sortfield { get; set; }
        [JsonProperty("sortorder")] public string Sortorder { get; set; }

    }
    public sealed class EventMessage : HttpSendMessageBase<EventModel>, IEventMessage
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

        public EventMessage(EventZabbixFilter filter)
        {
            this.Message =
                new EventModel
                {
                    Limit = null,
                    GroupIds = filter.GroupIds,
                    Acknowledged = filter.Acknowledged,
                    Sortfield = filter.Sortfield,
                    Sortorder = filter.Sortorder
                };
            this.Authentication();
        }
    }



    [Method(TypesEnum.EventAcknowledge)]
    public sealed class EventAcknowledgeModel
    {
        private SeverityAcknowledge? _severity;


        [JsonProperty("eventids")] public object EventIds { get; set; }
        [JsonProperty("action")] public ActionAcknowledge ActionAcknowledge { get; set; }
        [JsonProperty("message")] public string MessageEvent { get; set; }
        [JsonProperty("severity")]
        public SeverityAcknowledge? SeverityAcknowledge
        {
            get { return _severity; }
            set
            {
                if (ActionAcknowledge == ActionAcknowledge.ChangeSeverity)
                    _severity = value;
                else
                    _severity = null;

            }
        }
    }
    public sealed class EventAcknowledgeMessage : HttpSendMessageBase<EventAcknowledgeModel>, IEventMessage
    {
        public EventAcknowledgeMessage(object eventIds, ActionAcknowledge actionacknowledge, string messageEvent = "", SeverityAcknowledge? severity = null)
        {
            this.Message =
                new EventAcknowledgeModel
                {
                    EventIds = eventIds,
                    ActionAcknowledge = actionacknowledge,
                    MessageEvent = messageEvent,
                    SeverityAcknowledge = severity
                };
            this.Authentication();
        }
    }


}
