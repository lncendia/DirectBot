using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DirectBot.BLL.HangfireAuthorization;

public class NoAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}