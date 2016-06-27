using System.Collections.Generic;
using System.IO;
using DeploymentManager.Core;
using DeploymentManager.WorkerProcess.Runners;

namespace DeploymentManager.WorkerProcess.Tasks
{
    public class RunPublishProfile : IDeploymentTask
    {
        public TaskResult RunTask(DeploymentContext deploymentContext)
        {
            string solutionFile = deploymentContext.TaskParameters.GetValue<RunPublishProfile>("solutionFile");
            string solutionPath = Path.Combine(deploymentContext.GitDirectory, solutionFile);

            string publishProfile = deploymentContext.TaskParameters.GetValue<RunPublishProfile>("publishProfile");

            MsBuildRunner msBuildRunner = new MsBuildRunner(deploymentContext.CurrentLogger);
            bool success = msBuildRunner.StartMsDeployProcess(solutionPath, null, "DeployOnBuild=true", "PublishProfile=" + publishProfile);

            if (!success)
            {
                return TaskResult.CreateTotalFailureResult("Cannot build publish profile.");
            }

            string packageFile = deploymentContext.TaskParameters.GetValue<RunPublishProfile>("packageFile");
            string packagePath = Path.Combine(deploymentContext.GitDirectory, packageFile);
            deploymentContext.DeploymentArtifacts.AddArtifact("DeployPackage", packagePath);

            return TaskResult.CreateSuccessfulResult();
        }

    }
}