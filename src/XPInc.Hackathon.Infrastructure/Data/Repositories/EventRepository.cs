using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Repositories;
using XPInc.Hackathon.Core.Domain.Strategies;
using static XPInc.Hackathon.Core.Domain.AlertDefinition;

namespace XPInc.Hackathon.Infrastructure.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        public async Task<IEnumerable<AlertDefinition>> GetAlertDefinitionsAsync(CancellationToken cancellationToken)
        {
            var items = new List<AlertDefinition>();

            items.Add(new AlertDefinition()
            {
                EventLevel = new DisasterLevel(),
                Staggering = new List<StaggeringItem>
                {
                new StaggeringItem
                {
                    Index = 0,
                    ExpirationTime = TimeSpan.FromMinutes(5),
                    TeamMembers = new List<Guid>{Guid.NewGuid()}
                },
                new StaggeringItem
                {
                    Index =1,
                    ExpirationTime = TimeSpan.FromMinutes(10),
                      TeamMembers = new List<Guid>{Guid.NewGuid()}
                }
                }
            });

            items.Add(new AlertDefinition()
            {
                EventLevel = new HighLevel(),
                Staggering = new List<StaggeringItem>
                {
                new StaggeringItem
                {
                    Index = 0,
                    ExpirationTime = TimeSpan.FromMinutes(10),
                    TeamMembers = new List<Guid>{Guid.NewGuid()}
                },
                new StaggeringItem
                {
                    Index =1,
                    ExpirationTime = TimeSpan.FromMinutes(10),
                      TeamMembers = new List<Guid>{Guid.NewGuid()}
                }
                }
            });

            await Task.CompletedTask;

            return items;
        }

        public Task<IEnumerable<Event>> GetEventsAsync(string teamId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Event> GetEventAsync(string teamId, string eventId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SaveEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
