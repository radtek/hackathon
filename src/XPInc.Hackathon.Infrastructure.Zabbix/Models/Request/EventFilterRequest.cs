namespace XPInc.Hackathon.Infrastructure.Zabbix.Models.Request
{
    public class EventFilterRequest
    {
        public int[] GroupIds { get; set; }
        public bool Acknowledged { get; set; } = false;
        public string[] Sortfield { get; set; } = new[] { "clock" };
        public string Sortorder { get; set; } = "asc"; // "asc | desc"
    }
}
