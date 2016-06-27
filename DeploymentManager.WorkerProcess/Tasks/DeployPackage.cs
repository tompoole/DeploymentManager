using System.Collections.Generic;
using System.Text;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess.Tasks
{
    public class DeployPackage : IDeploymentTask
    {
        public TaskResult RunTask(DeploymentContext deploymentContext)
        {
            DeploymentArtifact packageArtifact = deploymentContext.DeploymentArtifacts.GetArtifactByKey("DeployPackage");

            if (packageArtifact == null)
            {
                return TaskResult.CreateErrorResult("Cannot find deployment package artificat");
            }

            // Get task parameters
            TaskParameterManager taskParams = deploymentContext.TaskParameters;
            string siteName = taskParams.GetValue<DeployPackage>("siteName");
            string server = taskParams.GetValue<DeployPackage>("server");
            string username = taskParams.GetValue<DeployPackage>("username");
            string password = taskParams.GetValue<DeployPackage>("password");


            StringBuilder argumentsBuilder = new StringBuilder();
            argumentsBuilder.Append("-verb:sync ");

            argumentsBuilder.AppendFormat("-source:package='{0}' ", packageArtifact.Path);
            argumentsBuilder.AppendFormat("-dest:contentPath='{0}',wmsvc='{1}',username='{2}',password='{3}' ", siteName, server, username, password);

            argumentsBuilder.AppendFormat("-allowUntrusted ");
            argumentsBuilder.AppendFormat("-enableRule:DoNotDeleteRule");


            MsDeployRunner deployRunner = new MsDeployRunner(deploymentContext.CurrentLogger);

            bool success = deployRunner.Run(argumentsBuilder.ToString());

            if (!success)
            {
                return TaskResult.CreateTotalFailureResult("msdeploy (deploy) did not complete successfully.");
            }

            return TaskResult.CreateSuccessfulResult();
        }
    }
}