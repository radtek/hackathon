using XPInc.Hackathon.Framework.Streaming;

namespace XPInc.Hackathon.Infrastructure.Streaming
{
    public sealed class RedisStreamingConfiguration : IStreamingConfiguration
    {
        public string Endpoint { get; set; }

        public int Database { get; set; }
    }
}
