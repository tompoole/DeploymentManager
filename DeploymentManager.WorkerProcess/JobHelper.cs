using NLog;
using DeploymentManager.Core;
using DeploymentManager.Core.Entities;

namespace DeploymentManager.WorkerProcess
{
    public static class JobHelper
    {
        public static Job GetJob(int jobId, Logger defaultLogger)
        {
            Job currentJob;
            using (var repo = new JobRepository())
            {
                defaultLogger.Info("Passed job with ID of {0}", jobId);

                currentJob = repo.GetJobById(jobId);

                if (currentJob == null)
                {
                    defaultLogger.Warn("Job not found");
                    return null;
                }

                defaultLogger.Info("Job found. URL is {0} and branch is {1}", currentJob.Url, currentJob.Branch);

                if (currentJob.State != JobState.Pending)
                {
                    defaultLogger.Warn("Cannot start job. Current state is {0}", currentJob.State);
                    return null;
                }

                repo.UpdateStateForJob(currentJob, JobState.Running);
            }

            return currentJob;
        }
         

        public static void MarkJobAsComplete(Job job)
        {
            using (var repo = new JobRepository())
            {
                repo.UpdateStateForJob(job, JobState.Completed);
            }
        }

        public static void MarkJobAsCompleteWithError(Job job)
        {
            using (var repo = new JobRepository())
            {
                repo.UpdateStateForJob(job, JobState.CompletedWithError);
            }
        }

        public static void MarkJobAsFailed(Job job, string error)
        {
            using (var repo = new JobRepository())
            {
                repo.UpdateStateForJob(job, JobState.Failed, error);
            }
        }
    }
}