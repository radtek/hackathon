using System;
using System.Collections.Generic;
using System.Linq;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class Workflow
    {
        private readonly List<TeamMember> _members = new List<TeamMember>();

        public Event Event { get; private set; }

        public IReadOnlyCollection<TeamMember> Members => _members;

        private Workflow()
        { }

        public static Workflow Create(CreateWorkflowCommand command)
        {
            var workflow = new Workflow
            {
                Event = command.Event
            };

            var teamsWithGivenHost = command.Teams.Where(_ => _.Hosts.Contains(command.Event.Host));

            foreach (var team in teamsWithGivenHost)
            {
                var membersWithGivenSeverity = team.Members.Where(_ => _.Levels.Contains(command.Event.Severity));

                workflow._members.AddRange(membersWithGivenSeverity);
            }

            return workflow;
        }
    }
}
