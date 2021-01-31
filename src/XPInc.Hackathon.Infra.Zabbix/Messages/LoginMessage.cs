using Newtonsoft.Json;
using XPInc.Hackathon.Infra.Zabbix.Attributes;
using XPInc.Hackathon.Infra.Zabbix.Enums;

namespace XPInc.Hackathon.Infra.Zabbix.Messages
{
    [Method(Types.UserLogin)]
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
