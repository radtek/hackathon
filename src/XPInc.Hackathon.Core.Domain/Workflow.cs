using System;
using System.Collections.Generic;
using System.Linq;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class Workflow
    {
        private readonly List<TeamMember> _members = new List<TeamMember>();

        public Incident Incident { get; private set; }

        public IReadOnlyCollection<TeamMember> Members => _members;

        private Workflow()
        { }

        public static Workflow Create(CreateWorkflowCommand command)
        {
            var workflow = new Workflow
            {
                Incident = command.Incident
            };

            var teamsWithGivenHost = command.Teams.Where(_ => _.Hosts.Contains(command.Incident.Host));

            foreach (var team in teamsWithGivenHost)
            {
                var membersWithGivenSeverity = team.Members.Where(_ => _.Levels.Contains(command.Incident.Severity));

                workflow._members.AddRange(membersWithGivenSeverity);
            }

            return workflow;
        }
    }
}
