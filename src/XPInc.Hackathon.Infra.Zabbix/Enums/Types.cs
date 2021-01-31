using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Infra.Zabbix.Enums
{
    public enum Types
    {
        [Display(Name = "user.login")] UserLogin = 1,
        [Display(Name = "user.authenticate")] UserAuthenticate = 2,
        [Display(Name = "event.get")] Incidents = 3
    }
}
