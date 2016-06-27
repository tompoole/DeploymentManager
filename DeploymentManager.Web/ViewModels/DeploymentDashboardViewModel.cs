using System.Collections.Generic;

namespace DeploymentManager.Web.ViewModels
{
    public class DeploymentDashboardViewModel
    {
        public List<JobStatusViewModel> RunningDeployments { get; set; }
        public List<JobStatusViewModel> PendingDeployments { get; set; }
        public List<JobStatusViewModel> CompletedDeployments { get; set; }
    }
}