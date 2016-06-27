using System.Diagnostics;
using System.IO;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess.Tasks
{
    public class NugetRestore : IDeploymentTask
    {
        public TaskResult RunTask(DeploymentContext deploymentContext)
        {
            string nugetExePath = Path.Combine(Directory.GetCurrentDirectory(), @"nuget\nuget.exe");

            string solutionFile = deploymentContext.TaskParameters.GetValue<NugetRestore>("solutionFile");
            string solutionPath = Path.Combine(deploymentContext.GitDirectory, solutionFile);

            string parameters = string.Format("restore \"{0}\"", solutionPath);


            deploymentContext.CurrentLogger.Info("Nuget Exe: {0}", nugetExePath);
            deploymentContext.CurrentLogger.Info("Solution Path: {0}", solutionPath);

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = nugetExePath,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    Arguments = parameters,
                    UseShellExecute = false
                }
            };

            process.Start();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                return TaskResult.CreateErrorResult("Nuget restore failed.");
            }

            return TaskResult.CreateSuccessfulResult();
        }
    }
}