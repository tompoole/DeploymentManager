using System.Web.Http;
using DeploymentManager.Core;

namespace DeploymentManager.Web.Api
{
    public class DeployController : ApiController
    {
        [HttpGet]
        public string Create(string url, string branch)
        {
            using (var repo = new JobRepository())
            {
                repo.AddNewJob(url, branch);
            }

            return "Job Queued.";
        }
    }
}