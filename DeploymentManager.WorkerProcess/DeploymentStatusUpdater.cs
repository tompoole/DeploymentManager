using System;
using DeploymentManager.Core;

namespace DeploymentManager.WorkerProcess
{
    public static class DeploymentStatuses
    {
        public const string VscCheckout = "Checking files out from source control";
    }


    public class DeploymentStatusUpdater : IDisposable
    {
        private readonly DeploymentContext _context;
        private readonly JobRepository _taskRepository;

        public DeploymentStatusUpdater(DeploymentContext context)
        {
            _context = context;
            _taskRepository = new JobRepository();
        }

        public void UpdateStatus(string status)
        {
            _taskRepository.UpdateStatusForJob(_context.Job, status);
        }

        public void Dispose()
        {
            _taskRepository.Dispose();
        }
    }
}