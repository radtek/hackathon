using MediatR;

namespace XPInc.Hackathon.Framework.CQRS.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
        where TResponse : class
    { }
}
