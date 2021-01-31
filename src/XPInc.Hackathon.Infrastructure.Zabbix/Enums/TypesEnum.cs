using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Enums
{
    public enum TypesEnum
    {
        [Display(Name = "user.login")] UserLogin = 1,
        [Display(Name = "user.authenticate")] UserAuthenticate = 2,
        [Display(Name = "event.get")] Events = 3,
        [Display(Name = "event.acknowledge")] EventAcknowledge = 4
    }
}
