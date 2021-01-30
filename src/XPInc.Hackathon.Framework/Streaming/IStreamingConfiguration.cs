namespace XPInc.Hackathon.Framework.Streaming
{
    public interface IStreamingConfiguration
    {
        string Endpoint { get; set; }
        int Database { get; set; }
    }
}
