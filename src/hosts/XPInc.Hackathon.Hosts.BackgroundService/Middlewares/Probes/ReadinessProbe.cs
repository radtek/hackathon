using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XPInc.Hackathon.Hosts.BackgroundService.Middlewares
{
    public class ReadinessProbe : IHealthCheck
    {
        private volatile bool _taskCompleted = false;

        public void SignalTaskCompleted() => this._taskCompleted = true;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this._taskCompleted)
            {
                return Task.FromResult(HealthCheckResult.Healthy($"'{context.Registration.Name}' startup task is finished."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy($"'{context.Registration.Name}' startup task is still running."));
        }
    }
}
