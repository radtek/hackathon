using System.Collections.Generic;

namespace XPInc.Hackathon.Core.Application.Services
{
    public class EventAckRequest
    {
        public IEnumerable<string> EventIds { get; set; }

        public ActionTypes Action { get; set; }

        public string Message { get; set; }

        public enum ActionTypes
        {
            CloseProblem = 1,
            AcknowledgeEvent = 2,
            AddMessage = 4,
            ChangeSeverity = 8,
            UnacknowledgeEvent = 16
        }
    }
}
