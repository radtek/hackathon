using System.Threading;
using System.Threading.Tasks;

namespace XPInc.Hackathon.Core.Application
{
    public interface IIncidentService
    {
        Task AckAsync(CancellationToken cancellationToken = default);
    }
}