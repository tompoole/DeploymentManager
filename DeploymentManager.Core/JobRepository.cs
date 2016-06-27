using System;
using System.Collections.Generic;
using NPoco;
using DeploymentManager.Core.Entities;

namespace DeploymentManager.Core
{
    public class JobRepository : IDisposable
    {
        private readonly Database _database;

        public JobRepository()
        {
            _database = new Database("databaseConnection");
        }

        public Job AddNewJob(string url, string branch)
        {
            string name = url.Substring(url.LastIndexOf('/') + 1).Replace(".git", "");

            // trim branch name
            int index = branch.LastIndexOf('/');
            index = index < 0 ? 0 : index + 1;
            branch = branch.Substring(index);

            Job newJob = new Job()
                {
                    Name = name,
                    Url = url,
                    Branch = branch,
                    State = JobState.Pending,
                    DateCreated = DateTime.Now
                };

            _database.Insert(newJob);
            return newJob;
        }

        public List<Job> GetAllJobs()
        {
            return _database.Query<Job>().ToList();
        }


        public Job GetOldestUnstartedJob()
        {
            return _database.FirstOrDefault<Job>("WHERE State = @0 ORDER BY DateCreated ASC", (int)JobState.Pending);
        }

        public IList<Job> GetCurrentlyRunningJobs()
        {
            return _database.Fetch<Job>("WHERE State = @0 ORDER BY DateStarted ASC", (int) JobState.Running);
        }

        public IList<Job> GetPendingJobs()
        {
            return _database.Fetch<Job>("WHERE State = @0 ORDER BY DateCreated ASC", (int)JobState.Pending);
        }

        public IList<Job> GetCompletedJobs(int take)
        {
            return _database.Fetch<Job>("SELECT TOP (@0) * FROM Jobs WHERE State = @1 OR State = @2 OR State = @3 ORDER BY DateFinished DESC", 
                take,
                (int)JobState.Completed, 
                (int)JobState.CompletedWithError, 
                (int)JobState.Failed);
        }

        public void UpdateStateForJob(Job job, JobState state, string statusMessage = null)
        {
            job.State = state;

            if (job.State == JobState.Running)
            {
                job.DateStarted = DateTime.Now;
            }

            if (job.State == JobState.Completed || job.State == JobState.CompletedWithError || job.State == JobState.Failed)
            {
                job.DateFinished = DateTime.Now;
            }

            if (job.State == JobState.Completed)
            {
                job.StatusMessage = "";
            }

            if (!string.IsNullOrEmpty(statusMessage))
            {
                job.StatusMessage = statusMessage;
            }

            _database.Update(job);
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        public Job GetJobById(int jobId)
        {
            return _database.SingleOrDefaultById<Job>(jobId);
        }

        public void UpdateStatusForJob(Job job, string status)
        {
            job.StatusMessage = status;
            _database.Update(job);
        }
    }
}