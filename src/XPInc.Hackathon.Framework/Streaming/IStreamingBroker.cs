using System;
using System.Collections.Generic;

namespace XPInc.Hackathon.Framework.Streaming
{
    public interface IStreamingBroker : IDisposable
    {
        void CreateGroup(string key, string group);
        void Add<T>(string key, T value);
        IEnumerable<T> Read<T>(string key, string group, string consumer, int? countPerStream = default);
    }
}
