using Newtonsoft.Json;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Attributes;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Enums;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages
{
    [Method(TypesEnum.UserLogin)]
    public class LoginModel
    {
        [JsonProperty("user")] public string User { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
    }


    public sealed class LoginMessage : HttpSendMessageBase<LoginModel>
    {
        public LoginMessage(string user, string password)
        {
            this.Message =
                new LoginModel
                {
                    User = user,
                    Password = password
                };
        }
    }
}
