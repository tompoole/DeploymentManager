using System.Collections.Generic;

namespace DeploymentManager.Web.ViewModels
{
    public class CompletedJobsViewModel
    {
        public List<JobStatusViewModel> CompletedDeployments { get; set; }
    }
}