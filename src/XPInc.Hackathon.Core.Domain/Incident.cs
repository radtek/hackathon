using System;
using System.Collections.Generic;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class Incident
    {
        private readonly List<string> _tags = new List<string>();

        private readonly List<Action> _actions = new List<Action>();

        public Guid TrackId { get; private set; }

        public string EventId { get; private set; }

        public Level Severity { get; private set; }

        public string Trigger { get; private set; }

        public DateTimeOffset Time => DateTimeOffset.Now;

        public IncidentStatus Status { get; private set; }

        public DateTimeOffset? RecoveryTime { get; private set; }

        public string Host { get; private set; }

        public string ProblemDescription { get; private set; }

        public TimeSpan Duration => RecoveryTime.HasValue ? RecoveryTime.Value - Time : TimeSpan.Zero;

        public bool Acknowledge { get; private set; }

        public IReadOnlyCollection<string> Tags => _tags;

        public IReadOnlyCollection<Action> Actions => _actions;

        private Incident()
        { }

        public static Incident Create(CreateIncidentCommand command)
        {
            var incident = new Incident
            {
                TrackId = Guid.NewGuid(),
                EventId = command.EventId,
                Severity = command.Severity,
                Trigger = command.Trigger,
                Status = IncidentStatus.Problem,
                Host = command.Host,
                ProblemDescription = command.ProblemDescription
            };

            incident._tags.AddRange(command.Tags); // add tags if any

            var actionCommand = new CreateActionCommand
            {
                Username = command.Username,
                Type = ActionType.Created,
                Message = command.ProblemDescription,
                Status = ActionStatus.Sent
            };
            var action = Action.Create(actionCommand); // create a new action along with this incident

            incident._actions.Add(action);

            return incident;
        }

        public void AddAction(Action action)
        {
            _actions.Add(action);
        }

        public void Resolve(ResolveIncidentCommand command)
        {
            Status = IncidentStatus.Resolved;
            RecoveryTime = DateTimeOffset.Now;

            var actionCommand = new CreateActionCommand
            {
                Username = command.Username,
                Type = ActionType.Resolved,
                Message = command.Message,
                Status = ActionStatus.Sent
            };
            var action = Action.Create(actionCommand); // create a new action to this incident

            _actions.Add(action);
        }

        public void Ack(AckIncidentCommand command)
        {
            var actionCommand = new CreateActionCommand
            {
                Username = command.Username,
                Type = ActionType.None,
                Message = command.Message,
                Status = ActionStatus.Sent
            };
            var action = Action.Create(actionCommand); // create a new action to this incident

            _actions.Add(action);
        }
    }
}