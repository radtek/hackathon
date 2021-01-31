using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class AlertDefinition
    {
        public Event Incident { get; private set; }

        public Workflow Workflow { get; private set; }

        private AlertDefinition()
        { }

        public static AlertDefinition Create(CreateAlertDefinitionCommand command)
        {
            var alertDefinition = new AlertDefinition
            {
                Incident = command.Incident
            };
            var workflowCommand = new CreateWorkflowCommand
            {
                Incident = command.Incident
            };

            alertDefinition.Workflow = Workflow.Create(workflowCommand); // create a new workflow

            return alertDefinition;
        }
    }
}
