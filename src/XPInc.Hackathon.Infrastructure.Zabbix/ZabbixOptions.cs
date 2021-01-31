using System;

namespace XPInc.Hackathon.Infrastructure.Zabbix
{
    public sealed class ZabbixOptions
    {
        public const string Section = "Zabbix";

        public string Endpoint { get; set; }

        public int? RetryCount { get; set; }

        public int? SleepSecondsPow { get; set; }

        public int? ExceptionsBeforeBreak { get; set; }

        public TimeSpan? DurationOfBreak { get; set; }
    }
}
