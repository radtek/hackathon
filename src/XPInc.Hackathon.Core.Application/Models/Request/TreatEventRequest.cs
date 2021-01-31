using System.Runtime.Serialization;

namespace XPInc.Hackathon.Core.Application.Models.Request
{
    public enum TreatEventRequestType
    {
        [EnumMember(Value = "Lido")]
        Ack = 1,
        [EnumMember(Value = "Fechado")]
        Close = 2
    }

    public sealed class TreatEventRequest
    {
        public string Username { get; set; }

        public string Message { get; set; }

        public TreatEventRequestType Type { get; set; }
    }
}
