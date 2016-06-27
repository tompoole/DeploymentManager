using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using DeploymentManager.Core;
using DeploymentManager.Core.Entities;
using DeploymentManager.Web.ViewModels;

namespace DeploymentManager.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new DeploymentDashboardViewModel();

            using (var repo = new JobRepository())
            {
                IList<Job> allRunningJobs = repo.GetCurrentlyRunningJobs();
                viewModel.RunningDeployments = allRunningJobs.Select(GetViewModelForJob).ToList();

                IList<Job> pendingJobs = repo.GetPendingJobs();
                viewModel.PendingDeployments = pendingJobs.Select(GetViewModelForJob).ToList();

                IList<Job> completedJobs = repo.GetCompletedJobs(10);
                viewModel.CompletedDeployments = completedJobs.Select(GetViewModelForJob).ToList();

            }

            return View(viewModel);
        }


        public ActionResult MoreCompleted()
        {
            var viewModel = new CompletedJobsViewModel();

            using (var repo = new JobRepository())
            {
                IList<Job> completedJobs = repo.GetCompletedJobs(100);
                viewModel.CompletedDeployments = completedJobs.Select(GetViewModelForJob).ToList();
            }

            return View(viewModel);
        }

        private JobStatusViewModel GetViewModelForJob(Job job)
        {
            TimeSpan? timeSpan = null;
            if (job.DateFinished.HasValue)
            {
                timeSpan = job.DateFinished.Value.Subtract(job.DateStarted.Value);
            }
            else if (job.DateStarted.HasValue)
            {
                timeSpan = DateTime.Now.Subtract(job.DateStarted.Value);
            }

            return new JobStatusViewModel
                {
                    Id = job.Id,
                    JobName = job.Name,
                    Url = job.Url,
                    State = job.State.ToString(),
                    Status = job.StatusMessage,
                    Branch = job.Branch,
                    CreatedDate = job.DateCreated,
                    CompletedDate = job.DateFinished,
                    StartedDate = job.DateStarted,
                    TimeTaken = timeSpan
                };
        }

        private readonly string _logDirectory = ConfigurationManager.AppSettings["LogsDirectory"];

        public ActionResult ViewLog(int id)
        {
            Job job;
            using (var repo = new JobRepository())
            {
                job = repo.GetJobById(id);
            }

            if (job == null)
            {
                return Content("Job not found.");
            }

            string logFileName = "Deploy_" + job.Name + "_" + job.Id + ".log";

            string logFile = Path.Combine(_logDirectory, logFileName);
            if (System.IO.File.Exists(logFile))
            {
                string allText = System.IO.File.ReadAllText(logFile);
                var viewModel = new DeploymentLogViewModel
                    {
                        JobId = job.Id.ToString(),
                        DeploymentLog = allText,
                        LogName = job.Name
                    };

                return View(viewModel);
            }


            return Content("File not found.");

        }

    }
}