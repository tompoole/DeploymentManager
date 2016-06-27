using DeploymentManager.Core;
using DeploymentManager.WorkerProcess.Runners;

namespace DeploymentManager.WorkerProcess.Tasks
{
    public class TransformWebConfig : IDeploymentTask
    {
        public TaskResult RunTask(DeploymentContext deploymentContext)
        {
            string configValue = "configuration=\"" + deploymentContext.Branch + "\"";
            string wwwDir = "WWWPath=\"" + deploymentContext.GitDirectory + @"\www""";

            MsBuildRunner msBuildRunner = new MsBuildRunner(deploymentContext.CurrentLogger);
            bool success = msBuildRunner.StartMsDeployProcess("TransformWebConfig.build", "TransformWebConfig", configValue, wwwDir);

            if (!success)
            {
                return TaskResult.CreateErrorResult("msbuild did not complete successfully.");
            }

            return TaskResult.CreateSuccessfulResult();
        }
    }
}