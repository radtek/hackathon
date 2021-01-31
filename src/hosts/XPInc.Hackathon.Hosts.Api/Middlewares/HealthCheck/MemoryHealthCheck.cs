using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XPInc.Hackathon.Hosts.Api.Configuration;

namespace XPInc.Hackathon.Hosts.Api.Middlewares
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly ApiHealthOptions _options;

        public MemoryHealthCheck([FromServices] IOptionsMonitor<ApiHealthOptions> options) => this._options = options.CurrentValue;

        public string Name => "max-memory-usage-check";

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!this._options.Memory.Threshold.HasValue)
            {
                Task.FromResult(
                    new HealthCheckResult(HealthStatus.Healthy, "A threshold was not informed. Assuming everything is fine.")
                );
            }

            cancellationToken.ThrowIfCancellationRequested();

            long allocated = GC.GetTotalMemory(forceFullCollection: false);

            var data = new Dictionary<string, object>(capacity: 2)
            {
                {"allocated", allocated >> 10 >> 10}, // Micro optimization [statistically measured with BenchmarkDotNet (available in tests/Corporate.Coe.Position.Performance)]
                {"size", "MB"}
            };

            HealthStatus status = HealthStatus.Unhealthy;

            float sixtyFivePercentil = this._options.Memory.Threshold.Value * 0.70f;
            float ninetyFivePercentil = this._options.Memory.Threshold.Value * 0.95f;

            // Can not use "context.Registration.FailureStatus" because it's a sealed class and, therefore, can not be mocked.
            // Using manually HealthStatus.Unhealthy status.
            if (allocated < sixtyFivePercentil)
            {
                status = HealthStatus.Healthy;
            }
            else if (allocated > sixtyFivePercentil && allocated < ninetyFivePercentil)
            {
                status = HealthStatus.Degraded;
            }
            else if (allocated > ninetyFivePercentil)
            {
                status = HealthStatus.Unhealthy;
            }

            return Task.FromResult(
                new HealthCheckResult(status, $"Allocated bytes >= {_options.Memory.Threshold >> 10 >> 10}MB threshold.", null, data)
            );
        }
    }
}
