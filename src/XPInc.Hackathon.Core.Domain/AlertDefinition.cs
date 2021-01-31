using System;
using System.Collections.Generic;
using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class AlertDefinition
    {
        public Guid TeamId { get; set; }

        public EventLevel EventLevel { get; set; }

        public IEnumerable<StaggeringItem> Staggering { get; set; }

        public class StaggeringItem
        {
            public TimeSpan ExpirationTime { get; set; }

            public IEnumerable<Guid> TeamMembers { get; set; }
            public int Index { get; set; }
        }

        public AlertDefinition()
        { }

        public Workflow BuildNotifcationWorkflow()
        {
            throw new NotImplementedException();
        }

        public static AlertDefinition Create(CreateAlertDefinitionCommand command)
        {
            var alertDefinition = new AlertDefinition
            {
                //Incident = command.Event
            };
            var workflowCommand = new CreateWorkflowCommand
            {
                Event = command.Event
            };

            //alertDefinition.Workflow = Workflow.Create(workflowCommand); // create a new workflow

            return alertDefinition;
        }
    }
}
