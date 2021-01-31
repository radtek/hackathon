using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Repositories;
using XPInc.Hackathon.Framework.Streaming;

namespace XPInc.Hackathon.Hosts.BackgroundService.Jobs
{
    public class NotificationJob : IJob
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IStreamingBroker _streamBroker;

        public NotificationJob(IEventRepository eventRepository,
            ITeamRepository teamRepository,
            IStreamingBroker streamBroker)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _streamBroker = streamBroker ?? throw new ArgumentNullException(nameof(streamBroker));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _streamBroker.CreateGroup(AppConfig.DammedEventsStreamKey, AppConfig.EventsStreamGroupName);

            var events = _streamBroker.Read<Event>(AppConfig.DammedEventsStreamKey, AppConfig.EventsStreamGroupName, "notification_job", 100);
            var alertDefinitions = await _eventRepository.GetAlertDefinitionsAsync(context.CancellationToken);

            var notificationTasks = events.Select(evt => Task.Run(() =>
            {
                var lastAction = evt.Actions.LastOrDefault();
                var diff = DateTimeOffset.Now - lastAction.CreationDate;

                var alertDefinition = alertDefinitions.FirstOrDefault(a => a.EventLevel.Code == evt.Severity.Code);

                var staggeringItem = alertDefinition.Staggering.FirstOrDefault(a => diff.TotalMinutes < a.ExpirationTime.TotalMinutes);

                if (staggeringItem == null)
                {
                    return;
                }

                foreach (var teamMemberId in staggeringItem.TeamMembers)
                {
                    var notification = new Notification
                    {
                        EventId = evt.Id,
                        TeamId = evt.TeamId,
                        TeamMemberId = teamMemberId,
                        Text = evt.ProblemDescription,
                        Title = $"{evt.Severity.Code} - {evt.Host}",
                    };

                    _ = notification;

                    //TODO: Send to a notification provider (Push, SMS, E-mail, etc)
                }
            }));

            await Task.WhenAll(notificationTasks);
        }
    }
}
