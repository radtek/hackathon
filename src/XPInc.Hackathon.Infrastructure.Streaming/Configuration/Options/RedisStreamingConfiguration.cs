using XPInc.Hackathon.Framework.Streaming;

namespace XPInc.Hackathon.Infrastructure.Streaming
{
    public sealed class RedisStreamingOptions : IStreamingConfiguration
    {
        public static string SectionPath { get; } = "Streaming";

        public string Endpoint { get; set; }

        public int Database { get; set; }
    }
}
