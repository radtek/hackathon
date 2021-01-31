using System;
using System.Collections.Generic;
using System.Threading;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class Team
    {
        private readonly List<TeamMember> _members = new List<TeamMember>();

        private readonly List<string> _hosts = new List<string>();

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public DateTimeOffset CreateDate { get; private set; }

        public IReadOnlyCollection<TeamMember> Members => _members;

        public IReadOnlyCollection<string> Hosts => _hosts;

        public int ExternalId { get; set; }

        private Team()
        { }

        public static Team Create(CreateTeamCommand command) => new Team
        {
            Id = Guid.NewGuid(),
            ExternalId = command.ExternalId,
            Name = command.Name,
            CreateDate = DateTimeOffset.Now
        };

        public void AddMember(IEnumerable<TeamMember> members) => _members.AddRange(members);
    }
}
