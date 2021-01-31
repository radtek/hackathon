﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using XPInc.Hackathon.Core.Application.Services;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Events;
using XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers.Abstractions;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Core.Domain.Repositories;
using XPInc.Hackathon.Core.Domain.Strategies;
using static XPInc.Hackathon.Core.Domain.AlertDefinition;

namespace XPInc.Hackathon.Core.Application.UseCases.Commands.Handlers
{
    public sealed class CreateEventHandler : ICreateEventHandler
    {
        private readonly IEventService _eventService;
        private readonly IEventRepository _eventRepository;
        private readonly IMediator _mediatr;

        public CreateEventHandler(IEventService eventService, IEventRepository eventRepository, IMediator mediatr)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
        }

        public async Task<Unit> Handle(CreateEvent request, CancellationToken cancellationToken)
        {
            // bring groups

            var team = Team.Create(new Domain.Commands.CreateTeamCommand
            {
                Name = "NOC3X",
                ExternalId = 65
            });

            var teamMember1 = TeamMember.Create(new Domain.Commands.CreateTeamMemberCommand
            {
                Name = "Alisson Silveira",
                ExternalId = "U003198"
            });


            var teams = new[] { team };

            var internalEvent = new Event()
            {


            }

            var eventCreationTasks = teams.Select(team => Task.Run(async () =>
            {
                var externalEvents = await _eventService.GetEventsAsync(team.ExternalId, cancellationToken).ConfigureAwait(false);




                //var @event = new EventCreated(result);
                //_ = _mediatr.Publish(@event, cancellationToken); // fire event created (and forget)





                _eventRepository.SaveEventsAsync()

            });

            await Task.WhenAll(eventCreationTasks);


       

            return Unit.Value;
        }
    }
}
