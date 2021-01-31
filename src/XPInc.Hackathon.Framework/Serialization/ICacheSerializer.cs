using System.Threading;
using System.Threading.Tasks;

namespace XPInc.Hackathon.Framework.Serialization
{
    public interface ICacheSerializer<TType>
    {
        TTarget Deserialize<TTarget>(TType input)
            where TTarget : class;

        TType Serialize<TTarget>(TTarget input)
            where TTarget : class;
    }

    public interface ICacheSerializerAsync<TType>
    {
        ValueTask<TTarget> DeserializeAsync<TTarget>(TType input)
            where TTarget : class => this.DeserializeAsync<TTarget>(input, CancellationToken.None);

        ValueTask<TTarget> DeserializeAsync<TTarget>(TType input, CancellationToken cancellationToken)
            where TTarget : class;

        ValueTask<TType> SerializeAsync<TTarget>(TTarget input)
            where TTarget : class => this.SerializeAsync<TTarget>(input, CancellationToken.None);

        ValueTask<TType> SerializeAsync<TTarget>(TTarget input, CancellationToken cancellationToken)
            where TTarget : class;
    }
}
