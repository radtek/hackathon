using System;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class Action
    {
        public Guid TrackerId { get; private set; }

        public string Username { get; private set; }

        public DateTimeOffset CreationDate { get; private set; }

        public ActionType Type { get; private set; }

        public string Message { get; private set; }

        public ActionStatus Status { get; private set; }

        private Action()
        { }

        public static Action Create(CreateActionCommand command) => new Action
        {
            TrackerId = Guid.NewGuid(),
            Username = command.Username,
            CreationDate = DateTimeOffset.Now,
            Type = command.Type,
            Message = command.Message,
            Status = command.Status
        };
    }
}
