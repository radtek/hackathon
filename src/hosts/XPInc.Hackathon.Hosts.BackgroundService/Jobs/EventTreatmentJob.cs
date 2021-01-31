using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using XPInc.Hackathon.Core.Application.Services;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Repositories;
using XPInc.Hackathon.Framework.Streaming;

namespace XPInc.Hackathon.Hosts.BackgroundService.Jobs
{
    public class EventTreatmentJob : IJob
    {
        private readonly IEventRepository _eventRepository;
        private readonly IStreamingBroker _streamingBroker;
        private readonly IEventService _eventService;

        public EventTreatmentJob(IEventRepository eventRepository,
            IStreamingBroker streamingBroker,
            IEventService eventService)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _streamingBroker = streamingBroker;
            _eventService = eventService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var events = _streamingBroker.Read<Event>(AppConfig.TreatedEventsStreamKey, AppConfig.EventsStreamGroupName, nameof(EventTreatmentJob), 100);

            var treatmentTasks = events.Select(evt => Task.Run(() => { 
            
            

               
                _eventService.AckAsync()

            }));

            await Task.WhenAll(treatmentTasks);
        }
    }
}
