using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess
{
    public class DeploymentTaskRunner
    {
        public static bool RunTasks(DeploymentContext deploymentContext, DeploymentStatusUpdater statusUpdater)
        {
            IEnumerable<IDeploymentTask> tasks = GetTaskInstances(deploymentContext);
            Logger log = deploymentContext.CurrentLogger;

            foreach (IDeploymentTask task in tasks)
            {
                string taskName = task.GetType().Name;

                log.Info("Starting running task {0}", taskName);

                statusUpdater.UpdateStatus("Running task " + taskName);
                TaskResult result = task.RunTask(deploymentContext);

                switch (result.State)
                {
                    case TaskResultState.Completed:
                        log.Info("Task {0} ran sucessfully", task);
                        break;

                    case TaskResultState.CompletedWithError:
                        log.Error("Task {0} has errored: {1}", task, result.ErrorMessage);
                        break;

                    case TaskResultState.Failed:
                        log.Fatal("Task {0} has failed. Deployment job will now stop. Error is: {1}", task, result.ErrorMessage);
                        return false;
                }
            }

            // All tasks completed without failure, return true
            return true;
        }

        private const string TasksNamespace = "DeploymentManager.WorkerProcess.Tasks.";

        private static IEnumerable<IDeploymentTask> GetTaskInstances(DeploymentContext deploymentContext)
        {
            return deploymentContext
                .Configuration
                .Tasks
                .Select(name => Type.GetType(TasksNamespace + name, false, true))
                .Select(type => Activator.CreateInstance(type) as IDeploymentTask);
        }
    }
}