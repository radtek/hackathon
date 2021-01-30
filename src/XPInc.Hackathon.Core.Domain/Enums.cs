namespace XPInc.Hackathon.Core.Domain
{
    public enum IncidentStatus
    {
        None = 0,
        Problem = 1,
        Resolved = 2
    }

    public enum ActionType
    {
        None = 0,
        Created = 1,
        Alert = 2,
        Resolved = 3
    }

    public enum ActionStatus
    {
        None = 0,
        Failed = 1,
        Sent = 2
    }
}