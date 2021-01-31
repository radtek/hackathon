
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Infra.Zabbix.Response
{
    public class EventResponse : IZabbixHttpResponse
    {
        [JsonProperty("result")]
        public List<EventZaabix> Incidents { get; set; }


        public sealed class Event
        {
            private readonly List<string> _tags = new List<string>();

            private readonly List<Core.Domain.Action> _actions = new List<Core.Domain.Action>();

            public Guid TrackId { get; private set; }

            public string EventId { get; private set; }

            [JsonProperty("severity")]
            public Level Severity { get; private set; }

            public string Trigger { get; private set; }

            public DateTimeOffset Time => DateTimeOffset.Now;

            public IncidentStatus Status { get; private set; }

            public DateTimeOffset? RecoveryTime { get; private set; }

            [JsonProperty("source")]
            public string Host { get; private set; }

            [JsonProperty("name")]
            public string ProblemDescription { get; private set; }

            public TimeSpan Duration => RecoveryTime.HasValue ? RecoveryTime.Value - Time : TimeSpan.Zero;

            public bool Acknowledge { get; private set; }

            public Workflow Workflow { get; private set; }

            public IReadOnlyCollection<string> Tags => _tags;

            public IReadOnlyCollection<Core.Domain.Action> Actions => _actions;
        }


        public class EventZaabix
        {
            [JsonProperty("eventid")]
            public string EventId { get; set; }

            [JsonProperty("source")]
            public string Host { get; set; }

            [JsonProperty("object")]
            public string Object { get; set; }

            [JsonProperty("objectid")]
            public string ObjectId { get; set; }

            [JsonProperty("clock")]
            public string Clock { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("acknowledge")]
            public bool Acknowledge { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("severity")]
            public string Severity { get; set; }

            [JsonProperty("userid")]
            public string UserId { get; set; }

            public UrlModel[] Urls { get; set; }
        }

        public class UrlModel
        {
            [JsonProperty("url")] public string Url { get; set; }
            [JsonProperty("name")] public string Name { get; set; }
        }
    }
}
