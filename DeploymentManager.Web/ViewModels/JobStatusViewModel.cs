using System;

namespace DeploymentManager.Web.ViewModels
{
    public class JobStatusViewModel
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string Branch { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public TimeSpan? TimeTaken { get; set; }
    }
}