using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Runtime.Versioning;
using System.Security.AccessControl;

namespace HealthChecks;

[SupportedOSPlatform("windows")]
public class DirectoryHealthCheck : IHealthCheck
{
    private const string DirectoryPath = "C:\\";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var directoryInfo = new DirectoryInfo(DirectoryPath);
            var directoryAccessControl = directoryInfo.GetAccessControl();
            var accessRules = directoryAccessControl.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules is null)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    return Task.FromResult(HealthCheckResult.Healthy());
                }
            }

            return Task.FromResult(HealthCheckResult.Unhealthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Checking directory address is failed.", ex));
        }
    }
}
