using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Timers;
using Atlas;
using NLog;
using DeploymentManager.Core;
using DeploymentManager.Core.Entities;

namespace DeploymentManager.Service
{
    public class DeploymentManagerService : IAmAHostedProcess
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private Timer _timer;

        private const int MaxRunningJobs = 1;

        public void Start()
        {
            Logger.Info("Service starting...");

            double runIntervalSecs = double.Parse(ConfigurationManager.AppSettings["RunIntervalInSeconds"]);
            double runIntervalMs = TimeSpan.FromSeconds(runIntervalSecs).TotalMilliseconds;

            _timer = new Timer(runIntervalMs);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Job latestJob;
            IList<Job> currentlyRunningJobs;

            using (var jobRepo = new JobRepository())
            {
                currentlyRunningJobs = jobRepo.GetCurrentlyRunningJobs();
                latestJob = jobRepo.GetOldestUnstartedJob();
            }

            if (currentlyRunningJobs.Count >= MaxRunningJobs)
            {
                Logger.Debug("There are currently {0} running job(s). The max job count is {1}", currentlyRunningJobs.Count, MaxRunningJobs);
                return;
            }

            if (latestJob != null)
            {
                StartRunningJob(latestJob);
            }
        }

        private void StartRunningJob(Job job)
        {
            Logger.Info("Starting WorkerProcess for job {0}", job.Id);

            Process process = new Process
                {
                    StartInfo =
                        {
                            FileName = ConfigurationManager.AppSettings["WorkerProcessLocation"],
                            Arguments = string.Format("-i {0}", job.Id),
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            ErrorDialog = false,
                            RedirectStandardError = true
                        }
                };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Logger.Error("Worker process for job ID: {0} exited with error.", job.Id);
                //using (var repo = new JobRepository())
                //{
                //    //repo.UpdateStateForJob(job, JobState.CompletedWithError, "Unspecified error");
                //}
            }
            else
            {
                Logger.Info("Worker process for job ID: {0} completed.", job.Id);
            }

        }

        public void Stop()
        {
            Logger.Info("Service stopping...");
        }

        public void Resume()
        {
        }

        public void Pause()
        {
        }
    }
}