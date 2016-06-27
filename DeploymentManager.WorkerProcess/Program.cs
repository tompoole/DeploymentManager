using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using CommandLine;
using NLog;
using DeploymentManager.Core;
using DeploymentManager.Core.Entities;
using DeploymentManager.WorkerProcess.SourceControl;

namespace DeploymentManager.WorkerProcess
{
    class Program
    {
        private static Logger _defaultLogger;
        private static DeploymentStatusUpdater _statusUpdater;

        static void Main(string[] args)
        {
            // Setup 
            _defaultLogger = LogManager.GetLogger("Default");
            EnsureWorkingAreaExists();

            // Parse command line options
            CommandLineOptions options = new CommandLineOptions();
            Parser.Default.ParseArguments(args, options);

            Job currentJob = JobHelper.GetJob(options.JobId, _defaultLogger);
            if (currentJob == null) ExitProgramWithError("Job not found");

            // Intialise deployment context.
            DeploymentContext deploymentContext = DeploymentContextFactory.CreateContextForJob(currentJob);
            _statusUpdater = new DeploymentStatusUpdater(deploymentContext);

            // Checkout code from source control
            _statusUpdater.UpdateStatus(DeploymentStatuses.VscCheckout);
            GitProvider gitProvider = new GitProvider(deploymentContext);
            bool checkoutSuccess = gitProvider.Checkout();
            if (!checkoutSuccess) ExitProgramWithError("Git checkout was unsuccessful.", currentJob);

            // Load deployment conifguration
            bool configLoadSuccess = ConfigLoader.LoadConfigurationIntoContext(deploymentContext);
            if (!configLoadSuccess) ExitProgramWithError("Could not load configuration", currentJob);

            // Run Tasks
            bool taskRunResult = DeploymentTaskRunner.RunTasks(deploymentContext, _statusUpdater);

            if (taskRunResult)
            {
                _statusUpdater.Dispose();
                JobHelper.MarkJobAsComplete(currentJob);
            }
            else
            {
                ExitProgramWithError("Task did not complete.", currentJob);
            }
        }


        static void EnsureWorkingAreaExists()
        {
            // Set Current Directory to exe folder
            string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(workingDirectory);

            string workingArea = ConfigurationManager.AppSettings["WorkingAreaDirectory"];
            if(!Directory.Exists(workingArea))
            {
                ExitProgramWithError("Working directory could not be found");
            }
        }

        static void ExitProgramWithError(string error, Job job = null)
        {
            if (job != null)
            {
               JobHelper.MarkJobAsFailed(job,error); 
            }

            if (_statusUpdater != null)
            {
                _statusUpdater.Dispose();
            }
            
            _defaultLogger.Fatal(error);
            Environment.Exit(1);
        }

    }
}
