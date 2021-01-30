using XPInc.Hackathon.Core.Domain.Commands;

namespace XPInc.Hackathon.Core.Domain
{
    public sealed class AlertDefinition
    {
        public Incident Incident { get; private set; }

        public Workflow Workflow { get; private set; }

        private AlertDefinition()
        { }

        public static AlertDefinition Create(CreateAlertDefinitionCommand command)
        {
            var alertDefinition = new AlertDefinition
            {
                Incident = command.Incident
            };

            return alertDefinition;
        }
    }
}
