using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace XPInc.Hackathon.Framework.Serialization
{
    public sealed class CacheSerialization :
                            ICacheSerializer<string>,
                            ICacheSerializerAsync<string>,
                            ICacheSerializerAsync<byte[]>,
                            ICacheSerializerAsync<Stream>,
                            IBinarySerializer
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public TTarget Deserialize<TTarget>(string input)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            return JsonSerializer.Deserialize<TTarget>(input, this._options);
        }

        public ValueTask<TTarget> DeserializeAsync<TTarget>(string input, CancellationToken cancellationToken)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            return JsonSerializer.DeserializeAsync<TTarget>(stream, this._options, cancellationToken);
        }

        public ValueTask<TTarget> DeserializeAsync<TTarget>(Stream input, CancellationToken cancellationToken)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerializer.DeserializeAsync<TTarget>(input, this._options, cancellationToken);
        }

        public ValueTask<TTarget> DeserializeAsync<TTarget>(byte[] input, CancellationToken cancellationToken)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            using var stream = new MemoryStream(input);

            return JsonSerializer.DeserializeAsync<TTarget>(stream, this._options, cancellationToken);
        }

        public TTarget FromBinary<TTarget>(byte[] bytes)
            where TTarget : class
        {
            if (bytes == default)
            {
                return default;
            }

            if (bytes.Length == 0)
            {
                return default;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using var stream = new MemoryStream(bytes);

            // Client will be responsible for casting problems (performance).
#pragma warning disable S5773 // We're restricting type serialization with interfaces.
            return (TTarget)formatter.Deserialize(stream);
#pragma warning restore S5773 // We're restricting type serialization with interfaces.
        }

        public string Serialize<TTarget>(TTarget input)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            return JsonSerializer.Serialize<TTarget>(input);
        }

        public ValueTask<string> SerializeAsync<TTarget>(TTarget input)
            where TTarget : class => this.SerializeAsync<TTarget>(input, CancellationToken.None);

        public async ValueTask<string> SerializeAsync<TTarget>(TTarget input, CancellationToken cancellationToken)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            using var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync<TTarget>(stream, input, this._options, cancellationToken)
                                    .ConfigureAwait(false);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        async ValueTask<Stream> ICacheSerializerAsync<Stream>.SerializeAsync<TTarget>(TTarget input, CancellationToken cancellationToken)
        {
            if (input == default)
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Client will be responsible to dispose the stream.
            var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync<TTarget>(stream, input, this._options, cancellationToken)
                                    .ConfigureAwait(false);

            return stream;
        }

        async ValueTask<byte[]> ICacheSerializerAsync<byte[]>.SerializeAsync<TTarget>(TTarget input, CancellationToken cancellationToken)
        {
            if (input == default)
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            using var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync(stream, input, this._options, cancellationToken)
                                    .ConfigureAwait(false);

            return stream.ToArray();
        }

        public byte[] ToBinary<TTarget>(TTarget input)
            where TTarget : class
        {
            if (input == default)
            {
                return default;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using var stream = new MemoryStream();

            formatter.Serialize(stream, input);

            return stream.ToArray();
        }
    }
}
