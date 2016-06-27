using System.Configuration;
using System.IO;
using System.Reflection;
using NLog;
using DeploymentManager.Core;
using DeploymentManager.Core.Entities;

namespace DeploymentManager.WorkerProcess
{
    public class DeploymentContextFactory
    {
        public static DeploymentContext CreateContextForJob(Job job)
        {
            string name = job.Url.Substring(job.Url.LastIndexOf('/') + 1);
            name = name.Replace(".git", "");

            string workingDirectory = ConfigurationManager.AppSettings["WorkingAreaDirectory"];
            string projectDirectory = Path.Combine(workingDirectory, name);

            string gitDirectory = Path.Combine(projectDirectory, "git");
            string configDirectory = Path.Combine(gitDirectory, "deploy");
            string outputDirectory = Path.Combine(projectDirectory, "output");
            string webDirectory = Path.Combine(gitDirectory, "WWW");

            Logger logger = LogManager.GetLogger("Deploy_" + name + "_" + job.Id);

            var deploymentContext = new DeploymentContext
            {
                Name = name,
                Job = job,
                Branch = job.Branch,
                ConfigurationDirectory = configDirectory,

                ProjectDirectory = projectDirectory,
                GitDirectory = gitDirectory,
                OutputDirectory = outputDirectory,
                WebSiteDirectory = webDirectory,

                CurrentLogger = logger
            };

            return deploymentContext;
        }

        private static string GetBaseDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}