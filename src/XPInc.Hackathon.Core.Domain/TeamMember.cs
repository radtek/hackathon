using System;
using System.Collections.Generic;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class TeamMember
    {
        private readonly List<Level> _levels = new List<Level>();

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Username { get; private set; }

        public bool Enabled { get; private set; }

        public string Email { get; private set; }

        public string Phone { get; private set; }

        public TeamMember Manager { get; private set; }

        public IReadOnlyCollection<Level> Levels => _levels;

        private TeamMember()
        { }

        public static TeamMember Create(CreateTeamMemberCommand command) => new TeamMember
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Username = command.Username,
            Enabled = true,
            Email = command.Email,
            Phone = command.Phone,
            Manager = command.Manager
        };

        public void ChangeManager(TeamMember member) => Manager = member;
    }
}
