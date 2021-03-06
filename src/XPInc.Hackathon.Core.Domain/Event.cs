using System;
using System.Collections.Generic;
using System.Linq;
using XPInc.Hackathon.Core.Domain.Commands;
using static XPInc.Hackathon.Core.Domain.AlertDefinition;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class Event
    {
        private readonly List<string> _tags = new List<string>();

        private readonly List<Action> _actions = new List<Action>();

        public Guid Id { get; private set; }

        public string ExternalId { get; private set; }

        public EventLevel Severity { get; private set; }

        public string Trigger { get; private set; }

        public DateTimeOffset Time => DateTimeOffset.Now;

        public IncidentStatus Status { get; private set; }

        public DateTimeOffset? RecoveryTime { get; private set; }

        public string Host { get; private set; }

        public string ProblemDescription { get; private set; }

        public TimeSpan Duration => RecoveryTime.HasValue ? RecoveryTime.Value - Time : TimeSpan.Zero;

        public bool Acknowledge { get; private set; }

        public IReadOnlyCollection<Action> Actions => _actions;

        public Guid TeamId { get; set; }

        private Event()
        { }


        public static Event Create(CreateEventCommand command)
        {
            var incident = new Event
            {
                Id = Guid.NewGuid(),
                ExternalId = command.EventId,
                Severity = command.Severity,
                Trigger = command.Trigger,
                Status = IncidentStatus.Problem,
                Host = command.Host,
                ProblemDescription = command.ProblemDescription
            };

            if (command?.Tags?.Any() ?? false)
                incident._tags.AddRange(command?.Tags); // add tags if any

            var actionCommand = new CreateActionCommand
            {
                Username = command.Username,
                Type = ActionType.Created,
                Message = command.ProblemDescription,
                Status = ActionStatus.Sent
            };
            var action = Action.Create(actionCommand); // create a new action along with this incident

            incident._actions.Add(action);

            var workflowCommand = new CreateWorkflowCommand
            {
                Event = incident,
                Teams = command.Teams
            };
            var workflow = Workflow.Create(workflowCommand); // create the event workflow

           //incident.Workflow = workflow;

            return incident;
        }

        public void AddAction(Action action) => _actions.Add(action);

        public void Resolve(ResolveEventCommand command)
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

        public void Ack(AckEventCommand command)
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
