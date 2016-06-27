using System.Collections.Generic;
using System.IO;
using DeploymentManager.Core;
using DeploymentManager.WorkerProcess.Runners;

namespace DeploymentManager.WorkerProcess.Tasks
{
    public class PublishProjectToFolder : IDeploymentTask
    {
        public TaskResult RunTask(DeploymentContext deploymentContext)
        {
            string profileFile = deploymentContext.TaskParameters.GetValue<PublishProjectToFolder>("projectFile");

            string projectFilePath = Path.Combine(deploymentContext.GitDirectory, profileFile);
            string buildConfiguration = deploymentContext.TaskParameters.GetValue<PublishProjectToFolder>("buildConfiguration");

            MsBuildRunner msBuildRunner = new MsBuildRunner(deploymentContext.CurrentLogger);

            const string task = "Build;PipelinePreDeployCopyAllFilesToOneFolder";
            bool success = msBuildRunner.StartMsDeployProcess(projectFilePath, task, "Configuration=" + buildConfiguration);

            if (!success)
            {
                return TaskResult.CreateTotalFailureResult("Cannot publish to folder.");
            }


            string containingFolder = Path.GetDirectoryName(projectFilePath);
            string packageFolder = Path.Combine(containingFolder, "obj", buildConfiguration, @"Package\PackageTmp");

            deploymentContext.DeploymentArtifacts.AddArtifact("DeployPackageFolder", packageFolder);

            return TaskResult.CreateSuccessfulResult();
        }

    }
}