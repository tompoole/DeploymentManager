using System.IO;
using System.Text;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess.Tasks
{
    public class BuildPackage : IDeploymentTask
    {
        public TaskResult RunTask(DeploymentContext deploymentContext)
        {
            if (Directory.Exists(deploymentContext.OutputDirectory) == false)
            {
                Directory.CreateDirectory(deploymentContext.OutputDirectory);
            }


            DeploymentArtifact deployPackageFolder = deploymentContext.DeploymentArtifacts.GetArtifactByKey("DeployPackageFolder");
            string contentDir = deployPackageFolder != null ? deployPackageFolder.Path : deploymentContext.WebSiteDirectory;


            string packagePath = Path.Combine(deploymentContext.OutputDirectory, deploymentContext.Name + ".zip");

            StringBuilder argumentsBuilder = new StringBuilder();
            argumentsBuilder.Append("-verb:sync ");
            argumentsBuilder.AppendFormat("-source:contentPath='{0}' ", contentDir);
            argumentsBuilder.AppendFormat("-dest:package='{0}' ", packagePath);

            var msDeployHelper = new MsDeployRunner(deploymentContext.CurrentLogger);
            bool success = msDeployHelper.Run(argumentsBuilder.ToString());

            if (!success)
            {
                return TaskResult.CreateTotalFailureResult("msdeploy (package) did not complete successfully");
            }

            deploymentContext.DeploymentArtifacts.AddArtifact("DeployPackage", packagePath);

            return TaskResult.CreateSuccessfulResult();
        }
    }
}