using NLog;
using DeploymentManager.Core.Entities;

namespace DeploymentManager.Core
{
    public class DeploymentContext
    {
        public DeploymentContext()
        {
            DeploymentArtifacts = new DeploymentArtifactManager();
            TaskParameters = new TaskParameterManager();
        }

        public DeploymentConfiguration Configuration { get; set; }

        public Logger CurrentLogger { get; set; }

        public string Branch { get; set; }
        public string GitDirectory { get; set; }

        public TaskParameterManager TaskParameters { get; private set; }

        public DeploymentArtifactManager DeploymentArtifacts { get; private set; }

        public string ProjectDirectory { get; set; }

        public string OutputDirectory { get; set; }

        public string WebSiteDirectory { get; set; }

        public string Name { get; set; }

        public Job Job { get; set; }

        public string ConfigurationDirectory { get; set; }
    }
}