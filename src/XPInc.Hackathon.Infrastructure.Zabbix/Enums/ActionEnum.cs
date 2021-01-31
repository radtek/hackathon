using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Enums
{
    public enum ActionAcknowledge
    {
        [Display(Name = "Close Problem")] CloseProblem = 1,
        [Display(Name = "Acknowledge Event")] AcknowledgeEven = 2,
        [Display(Name = "Add Message")] AddMessage = 4,
        [Display(Name = "Change Severity")] ChangeSeverity = 8,
        [Display(Name = "Unacknowledge Event")] UnacknowledgeEvent = 16,
    }
}
