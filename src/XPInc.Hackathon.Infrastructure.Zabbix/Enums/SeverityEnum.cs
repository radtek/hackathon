using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Infrastructure.Zabbix.Enums
{
    public enum SeverityAcknowledge
    {
        [Display(Name = "Not Classified")] NotClassified = 0,
        [Display(Name = "Information")] Information = 1,
        [Display(Name = "Warning")] Warning = 2,
        [Display(Name = "Average")] Average = 3,
        [Display(Name = "High")] High = 4,
        [Display(Name = "Disaster")] Disaster = 5
    }
}
