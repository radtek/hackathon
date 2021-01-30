using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XPInc.Hackathon.Framework.Extensions;
using XPInc.Hackathon.Framework.Streaming;

namespace XPInc.Hackathon.Infrastructure.Streaming
{
    public sealed class RedisStreamingBroker : IStreamingBroker
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _cache;

        public RedisStreamingBroker(IStreamingConfiguration configuration)
        {
            var options = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                AllowAdmin = true
            };

            options.EndPoints.Add(configuration.Endpoint); //"jupiter.nossila.com:49157"

            _redis = ConnectionMultiplexer.Connect(options);
            _cache = _redis.GetDatabase(configuration.Database);
        }

        ~RedisStreamingBroker()
        {
            Dispose(false);
        }

        public void CreateGroup(string key, string group)
        {
            if (!_cache.KeyExists(key))
            {
                _cache.StreamCreateConsumerGroup(key, group);
            }
        }

        public void Add<T>(string key, T value)
        {
            _cache.StreamAdd(key, "content", value.ToJson());
        }

        public IEnumerable<T> Read<T>(string key, string group, string consumer, int? countPerStream = default)
        {
            var entry = _cache.StreamReadGroup(key, group, consumer, countPerStream);

            // ack entry response
            _cache.StreamAcknowledge(key, group, entry.Select(_ => _.Id).ToArray());

            return entry.Select(_ => Encoding.ASCII.GetString(_.Values.FirstOrDefault(_ => _.Name == "content").Value).FromJson<T>());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _redis != null)
            {
                _redis.Dispose();
            }
        }
    }
}
